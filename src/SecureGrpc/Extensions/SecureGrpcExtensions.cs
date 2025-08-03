using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace SecureGrpc.Extensions;

/// <summary>
/// Extension methods for easy integration with existing gRPC services
/// </summary>
public static class SecureGrpcExtensions
{
    /// <summary>
    /// Create secure call invoker for any gRPC channel
    /// </summary>
    /// <example>
    /// var channel = GrpcChannel.ForAddress("http://localhost:5001");
    /// var secureInvoker = channel.WithEncryption();
    /// var client = new YourService.YourServiceClient(secureInvoker);
    /// </example>
    public static CallInvoker WithEncryption(this GrpcChannel channel)
    {
        return channel.Intercept(new SecureClientInterceptor(channel));
    }
    
    /// <summary>
    /// Create a secure channel with fluent syntax
    /// </summary>
    public static SecureChannelBuilder CreateSecureChannel(this string host, int port)
    {
        return new SecureChannelBuilder(host, port);
    }
}

/// <summary>
/// Fluent builder for secure channels
/// </summary>
public class SecureChannelBuilder
{
    private readonly string _host;
    private readonly int _port;
    private readonly GrpcChannelOptions _options = new();
    
    /// <summary>
    /// Creates a new SecureChannelBuilder
    /// </summary>
    public SecureChannelBuilder(string host, int port)
    {
        _host = host;
        _port = port;
    }
    
    /// <summary>
    /// Sets a custom HTTP message handler
    /// </summary>
    public SecureChannelBuilder WithHttpHandler(HttpMessageHandler handler)
    {
        _options.HttpHandler = handler;
        return this;
    }
    
    /// <summary>
    /// Builds the gRPC channel
    /// </summary>
    public GrpcChannel Build()
    {
        return GrpcChannel.ForAddress($"http://{_host}:{_port}", _options);
    }
    
    /// <summary>
    /// Builds a secure call invoker with encryption
    /// </summary>
    public CallInvoker BuildSecure()
    {
        return Build().WithEncryption();
    }
}