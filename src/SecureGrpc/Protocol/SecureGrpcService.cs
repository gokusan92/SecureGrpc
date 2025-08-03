using Grpc.Core;
using Grpc.Net.Client;
using static SecureGrpc.Protocol.Messages;

namespace SecureGrpc.Protocol;

/// <summary>
/// gRPC service definition and implementation
/// </summary>
internal static class SecureGrpcService
{
    private const string ServiceName = "securegrpc.SecureService";
    
    private static readonly Method<KeyExchangeRequest, KeyExchangeReply> KeyExchangeMethod =
        new(MethodType.Unary, ServiceName, "KeyExchange",
            Marshallers.Create(
                r => r.ToByteArray(),
                KeyExchangeRequest.Parser.ParseFrom),
            Marshallers.Create(
                r => r.ToByteArray(),
                KeyExchangeReply.Parser.ParseFrom));
    
    private static readonly Method<SecureMessage, SecureMessage> SendSecureMethod =
        new(MethodType.Unary, ServiceName, "SendSecure",
            Marshallers.Create(
                r => r.ToByteArray(),
                SecureMessage.Parser.ParseFrom),
            Marshallers.Create(
                r => r.ToByteArray(),
                SecureMessage.Parser.ParseFrom));
    
    public static ServerServiceDefinition BindService(SecureServiceBase serviceImpl)
    {
        return ServerServiceDefinition.CreateBuilder()
            .AddMethod(KeyExchangeMethod, serviceImpl.KeyExchange)
            .AddMethod(SendSecureMethod, serviceImpl.SendSecure)
            .Build();
    }
    
    public abstract class SecureServiceBase
    {
        public abstract Task<KeyExchangeReply> KeyExchange(
            KeyExchangeRequest request, ServerCallContext context);
        
        public abstract Task<SecureMessage> SendSecure(
            SecureMessage request, ServerCallContext context);
    }
    
    public class SecureServiceClient : ClientBase<SecureServiceClient>
    {
        public SecureServiceClient(ChannelBase channel) : base(channel) { }
        
        protected SecureServiceClient(ClientBaseConfiguration configuration) 
            : base(configuration) { }
        
        protected override SecureServiceClient NewInstance(ClientBaseConfiguration configuration) 
            => new(configuration);
        
        public AsyncUnaryCall<KeyExchangeReply> KeyExchangeAsync(
            KeyExchangeRequest request, CallOptions options = default)
        {
            return CallInvoker.AsyncUnaryCall(KeyExchangeMethod, null, options, request);
        }
        
        public AsyncUnaryCall<SecureMessage> SendSecureAsync(
            SecureMessage request, CallOptions options = default)
        {
            return CallInvoker.AsyncUnaryCall(SendSecureMethod, null, options, request);
        }
    }
    
    private static class Marshallers
    {
        public static Marshaller<T> Create<T>(
            Func<T, byte[]> serializer,
            Func<byte[], T> deserializer)
        {
            return new Marshaller<T>(serializer, deserializer);
        }
    }
}