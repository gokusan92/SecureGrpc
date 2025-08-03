using System.Collections.Concurrent;
using Google.Protobuf;
using Grpc.Core;
using SecureGrpc.Crypto;
using static SecureGrpc.Protocol.Messages;

namespace SecureGrpc.Protocol;

/// <summary>
/// Server-side implementation of the secure service
/// </summary>
internal class SecureServiceImpl : SecureGrpcService.SecureServiceBase
{
    private readonly ConcurrentDictionary<string, SecureSession> _sessions = new();
    private readonly HybridCrypto _crypto = new();
    private readonly AesGcm _aes = new();
    
    public Func<byte[], byte[]>? MessageHandler { get; set; }
    public Func<byte[], Task<byte[]>>? AsyncMessageHandler { get; set; }
    
    public override Task<KeyExchangeReply> KeyExchange(
        KeyExchangeRequest request, ServerCallContext context)
    {
        // Generate server keys
        var (dhPub, mlkemPub, dhPriv, mlkemPriv) = _crypto.GenerateKeyPairs();
        
        // Compute shared secret
        var (mlkemCiphertext, sharedSecret) = _crypto.EncapsulateAndCompute(
            request.DhPublicKey.ToByteArray(),
            request.MlkemPublicKey.ToByteArray(),
            dhPriv);
        
        // Create session
        var sessionId = Guid.NewGuid().ToString();
        var session = new SecureSession(sessionId, sharedSecret);
        _sessions[sessionId] = session;
        
        return Task.FromResult(new KeyExchangeReply
        {
            SessionId = sessionId,
            DhPublicKey = ByteString.CopyFrom(dhPub),
            MlkemCiphertext = ByteString.CopyFrom(mlkemCiphertext)
        });
    }
    
    public override async Task<SecureMessage> SendSecure(
        SecureMessage request, ServerCallContext context)
    {
        var sessionId = context.RequestHeaders.GetValue("session-id");
        if (sessionId == null || !_sessions.TryGetValue(sessionId, out var session))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid session"));
        }
        
        if (!session.IsValid)
        {
            _sessions.TryRemove(sessionId, out _);
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Session expired"));
        }
        
        try
        {
            // Decrypt request
            var key = session.DeriveKey();
            var decrypted = _aes.Decrypt(request.EncryptedData.ToByteArray(), key);
            
            // Process message
            byte[] response;
            if (AsyncMessageHandler != null)
            {
                response = await AsyncMessageHandler(decrypted);
            }
            else if (MessageHandler != null)
            {
                response = MessageHandler(decrypted);
            }
            else
            {
                response = decrypted; // Echo by default
            }
            
            // Encrypt response
            var encrypted = _aes.Encrypt(response, key);
            
            return new SecureMessage
            {
                EncryptedData = ByteString.CopyFrom(encrypted)
            };
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Processing failed"));
        }
    }
}