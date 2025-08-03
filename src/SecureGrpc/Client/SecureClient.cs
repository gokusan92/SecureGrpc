using Grpc.Net.Client;
using SecureGrpc.Protocol;

namespace SecureGrpc;

/// <summary>
/// Secure gRPC client with automatic encryption
/// </summary>
public class SecureClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly SecureClientImpl _client;
    private SecureSession? _session;
    
    /// <summary>
    /// Initializes a new instance of the SecureClient class
    /// </summary>
    /// <param name="host">The host to connect to</param>
    /// <param name="port">The port to connect to</param>
    public SecureClient(string host, int port)
    {
        _channel = GrpcChannel.ForAddress($"http://{host}:{port}", new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
        _client = new SecureClientImpl(_channel);
    }
    
    /// <summary>
    /// Send data and get response - connects automatically if needed
    /// </summary>
    public async Task<byte[]> SendAsync(byte[] data)
    {
        if (_session == null)
        {
            _session = await _client.ConnectAsync();
        }
        
        return await _client.SendSecureAsync(data, _session);
    }
    
    /// <summary>
    /// Send string and get string response
    /// </summary>
    public async Task<string> SendAsync(string message)
    {
        var response = await SendAsync(System.Text.Encoding.UTF8.GetBytes(message));
        return System.Text.Encoding.UTF8.GetString(response);
    }
    
    /// <summary>
    /// Shutdown the client connection
    /// </summary>
    public Task ShutdownAsync()
    {
        _channel.Dispose();
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Disposes the client and releases resources
    /// </summary>
    public void Dispose()
    {
        _channel?.Dispose();
    }
}