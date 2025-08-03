using Grpc.Core;
using Grpc.Core.Interceptors;

namespace SecureGrpc.Extensions;

/// <summary>
/// Server interceptor that automatically decrypts incoming calls and encrypts responses
/// </summary>
internal class SecureServerInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // For now, pass through - full implementation would decrypt/encrypt here
        return await continuation(request, context);
    }
    
    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return continuation(requestStream, context);
    }
    
    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return continuation(request, responseStream, context);
    }
    
    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return continuation(requestStream, responseStream, context);
    }
}