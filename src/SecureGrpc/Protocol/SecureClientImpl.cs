using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using SecureGrpc.Crypto;
using static SecureGrpc.Protocol.Messages;

namespace SecureGrpc.Protocol;

/// <summary>
/// Client-side implementation of secure communication
/// </summary>
internal class SecureClientImpl
{
    private readonly SecureGrpcService.SecureServiceClient _client;
    private readonly HybridCrypto _crypto = new();
    private readonly AesGcm _aes = new();
    
    public SecureClientImpl(GrpcChannel channel)
    {
        _client = new SecureGrpcService.SecureServiceClient(channel);
    }
    
    public async Task<SecureSession> ConnectAsync()
    {
        // Generate client keys
        var (dhPub, mlkemPub, dhPriv, mlkemPriv) = _crypto.GenerateKeyPairs();
        
        // Send key exchange request
        var reply = await _client.KeyExchangeAsync(new KeyExchangeRequest
        {
            ClientId = Guid.NewGuid().ToString(),
            DhPublicKey = ByteString.CopyFrom(dhPub),
            MlkemPublicKey = ByteString.CopyFrom(mlkemPub)
        });
        
        // Compute shared secret
        var sharedSecret = _crypto.ComputeSharedSecret(
            dhPriv,
            reply.DhPublicKey.ToByteArray(),
            mlkemPriv,
            reply.MlkemCiphertext.ToByteArray());
        
        return new SecureSession(reply.SessionId, sharedSecret);
    }
    
    public async Task<byte[]> SendSecureAsync(byte[] data, SecureSession session)
    {
        // Encrypt data
        var key = session.DeriveKey();
        var encrypted = _aes.Encrypt(data, key);
        
        // Send request with session ID in metadata
        var metadata = new Metadata { { "session-id", session.Id } };
        var reply = await _client.SendSecureAsync(
            new SecureMessage { EncryptedData = ByteString.CopyFrom(encrypted) },
            new CallOptions(metadata));
        
        // Decrypt response
        return _aes.Decrypt(reply.EncryptedData.ToByteArray(), key);
    }
}