namespace SecureGrpc;

/// <summary>
/// Main entry point for SecureGrpc - dead simple secure gRPC
/// </summary>
public static class Secure
{
    /// <summary>
    /// Create a secure server on specified port
    /// </summary>
    /// <example>
    /// using var server = Secure.Server(5001)
    ///     .OnMessage(data => ProcessData(data))
    ///     .Start();
    /// </example>
    public static SecureServer Server(int port) => new(port);
    
    /// <summary>
    /// Create a secure client connection
    /// </summary>
    /// <example>
    /// using var client = Secure.Client("localhost", 5001);
    /// var response = await client.SendAsync("Hello!");
    /// </example>
    public static SecureClient Client(string host, int port) => new(host, port);
}