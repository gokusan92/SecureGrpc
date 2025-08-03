using SecureGrpc;
using SecureGrpc.Extensions;

// Security vulnerability check
Console.WriteLine("=== Security Vulnerability Check ===");
SecurityTestExample.Demo();

// Example usage demonstrations
Console.WriteLine("\n=== SecureGrpc Usage Examples ===\n");

// Example 1: Creating secure channels
Console.WriteLine("Example 1: Creating a secure gRPC channel");
var channel = "localhost".CreateSecureChannel(5001).Build();
Console.WriteLine($"✅ Created secure channel to localhost:5001");

// Example 2: Using the fluent API
Console.WriteLine("\nExample 2: Using fluent API with custom HTTP handler");
var secureChannel = "localhost".CreateSecureChannel(5001)
    .WithHttpHandler(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    })
    .Build();
Console.WriteLine("✅ Created channel with custom HTTP handler");

// Example 3: Extension method usage
Console.WriteLine("\nExample 3: Adding encryption to existing channels");
Console.WriteLine("Example code:");
Console.WriteLine("  var encrypted = channel.WithEncryption();");
Console.WriteLine("  var client = new YourService.YourServiceClient(encrypted);");

// Show WithEncryption examples
WithEncryptionExample.Demo();

Console.WriteLine("\n✅ All examples completed successfully!");
Console.WriteLine("\n📝 Note: For full server implementation, use ASP.NET Core with Grpc.AspNetCore");
Console.WriteLine("   This ensures you're using the secure, maintained gRPC implementation.");