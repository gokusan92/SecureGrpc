using FluentAssertions;
using Xunit;

namespace SecureGrpc.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class IntegrationTests : IAsyncLifetime
{
    private SecureServer? _server;
    private SecureClient? _client;
    private const int TestPort = 50123;
    
    public async Task InitializeAsync()
    {
        _server = Secure.Server(TestPort)
            .OnMessage(data =>
            {
                var message = System.Text.Encoding.UTF8.GetString(data);
                return System.Text.Encoding.UTF8.GetBytes($"Echo: {message}");
            })
            .Start();
        
        await Task.Delay(100); // Give server time to start
        
        _client = Secure.Client("localhost", TestPort);
    }
    
    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_server != null)
            await _server.ShutdownAsync();
    }
    
    [Fact]
    public async Task SendAsync_WithString_ShouldEchoBack()
    {
        // Arrange
        var message = "Hello, secure world!";
        
        // Act
        var response = await _client!.SendAsync(message);
        
        // Assert
        response.Should().Be($"Echo: {message}");
    }
    
    [Fact]
    public async Task SendAsync_WithBytes_ShouldEchoBack()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        
        // Act
        var response = await _client!.SendAsync(data);
        
        // Assert
        var expected = System.Text.Encoding.UTF8.GetBytes($"Echo: {System.Text.Encoding.UTF8.GetString(data)}");
        response.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task SendAsync_MultipleTimes_ShouldReuseSession()
    {
        // Act
        var response1 = await _client!.SendAsync("Message 1");
        var response2 = await _client!.SendAsync("Message 2");
        var response3 = await _client!.SendAsync("Message 3");
        
        // Assert
        response1.Should().Be("Echo: Message 1");
        response2.Should().Be("Echo: Message 2");
        response3.Should().Be("Echo: Message 3");
    }
}