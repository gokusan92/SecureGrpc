using Grpc.Net.Client;
using SecureGrpc.Extensions;

/// <summary>
/// Examples demonstrating WithEncryption() middleware usage
/// </summary>
public static class WithEncryptionExample
{
    public static void Demo()
    {
        Console.WriteLine("\n=== WithEncryption() Middleware Examples ===\n");
        
        // Example 1: Client-side encryption
        Console.WriteLine("1. Adding encryption to a gRPC client:");
        Console.WriteLine(@"
// Create a standard gRPC channel
var channel = GrpcChannel.ForAddress(""http://localhost:5001"");

// Add encryption using the extension method
var secureInvoker = channel.WithEncryption();

// Use with any gRPC client
var client = new YourService.YourServiceClient(secureInvoker);
");

        // Example 2: Fluent API usage
        Console.WriteLine("2. Using the fluent API for secure channels:");
        Console.WriteLine(@"
// Build a secure channel with fluent syntax
var secureInvoker = ""localhost""
    .CreateSecureChannel(5001)
    .BuildSecure();
    
// Ready to use with your gRPC client
var client = new YourService.YourServiceClient(secureInvoker);
");

        // Example 3: Custom configuration
        Console.WriteLine("3. Advanced configuration with HTTP handler:");
        Console.WriteLine(@"
// Configure HTTP handler for custom certificates or proxy
var channel = ""localhost""
    .CreateSecureChannel(5001)
    .WithHttpHandler(new HttpClientHandler
    {
        // Custom certificate validation
        ServerCertificateCustomValidationCallback = 
            (sender, cert, chain, errors) => ValidateCertificate(cert),
            
        // Proxy configuration
        Proxy = new WebProxy(""http://proxy:8080""),
        UseProxy = true
    })
    .Build();
");

        // Example 4: Server-side notes
        Console.WriteLine("4. Server-side implementation (ASP.NET Core):");
        Console.WriteLine(@"
// In Program.cs or Startup.cs:
var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc();

// Add your service implementation
builder.Services.AddSingleton<YourServiceImpl>();

var app = builder.Build();

// Map gRPC service
app.MapGrpcService<YourServiceImpl>();

app.Run();
");

        Console.WriteLine("\n📚 Key Points:");
        Console.WriteLine("   • WithEncryption() adds transparent encryption to any gRPC channel");
        Console.WriteLine("   • Works with existing gRPC service definitions");
        Console.WriteLine("   • No vulnerable Grpc.Core dependency - uses secure Grpc.Net.Client");
        Console.WriteLine("   • Compatible with ASP.NET Core gRPC servers");
    }
}