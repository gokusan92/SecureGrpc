# SecureGrpc Integration Guide

## Method 1: Local NuGet Package (Recommended for now)

Since the package isn't published to NuGet.org yet, you can use the local package:

### 1. Copy the NuGet package
```bash
# The package is located at:
/home/anonymous/RiderProjects/encsample/encsample/src/SecureGrpc/bin/Debug/SecureGrpc.1.0.0.nupkg
```

### 2. Add local NuGet source to your project
Create a `nuget.config` in your project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="local" value="/path/to/your/local/packages" />
  </packageSources>
</configuration>
```

### 3. Install the package
```bash
dotnet add package SecureGrpc --version 1.0.0 --source /path/to/local/packages
```

## Method 2: Project Reference

### 1. Clone/Copy the SecureGrpc source
```bash
# Copy the src/SecureGrpc folder to your solution
cp -r /home/anonymous/RiderProjects/encsample/encsample/src/SecureGrpc ~/YourProject/libs/SecureGrpc
```

### 2. Add project reference
```bash
dotnet add reference libs/SecureGrpc/SecureGrpc.csproj
```

## Method 3: Direct DLL Reference

### 1. Copy the built DLL
```bash
# Copy from:
/home/anonymous/RiderProjects/encsample/encsample/src/SecureGrpc/bin/Debug/net9.0/SecureGrpc.dll
```

### 2. Add to your project
```xml
<ItemGroup>
  <Reference Include="SecureGrpc">
    <HintPath>libs/SecureGrpc.dll</HintPath>
  </Reference>
</ItemGroup>
```

## Usage Example

Once installed, here's how to use it:

### Server
```csharp
using SecureGrpc;

// Create secure server
using var server = Secure.Server(5001)
    .OnMessage(data => {
        Console.WriteLine($"Received: {Encoding.UTF8.GetString(data)}");
        return Encoding.UTF8.GetBytes("Hello from secure server!");
    })
    .Start();

Console.WriteLine("Secure server running on port 5001");
Console.ReadLine();
```

### Client
```csharp
using SecureGrpc;

// Create secure client
using var client = Secure.Client("localhost", 5001);

// Send encrypted message
var response = await client.SendAsync("Hello secure server!");
Console.WriteLine($"Response: {response}");
```

### With Existing gRPC Services
```csharp
using SecureGrpc.Extensions;

// Add encryption to existing channel
var channel = new Channel("localhost", 5001, ChannelCredentials.Insecure)
    .WithEncryption();
    
var client = new YourService.YourServiceClient(channel);
```

## Dependencies

The library requires these NuGet packages:
- Grpc.Core (2.46.6) - ⚠️ Has security vulnerabilities
- Google.Protobuf (3.28.3)
- BouncyCastle.Cryptography (2.4.0)

## Important Security Note

⚠️ **WARNING**: This library uses Grpc.Core which has known security vulnerabilities (CVE-2023-32731, CVE-2023-33953). For production use, consider waiting for a version that uses Grpc.Net.Client.

## Troubleshooting

1. **Build errors**: Make sure you're using .NET 9.0
2. **Runtime errors**: Ensure all dependencies are restored
3. **Port conflicts**: Change the port if 5001 is already in use

## Complete Example Project

Create a new console app:

```bash
dotnet new console -n SecureGrpcDemo
cd SecureGrpcDemo
```

Add the package (using local source):

```bash
dotnet add package SecureGrpc --version 1.0.0 --source /path/to/local/packages
```

Replace Program.cs:

```csharp
using SecureGrpc;
using System.Text;

// Run server in background
var serverTask = Task.Run(async () =>
{
    using var server = Secure.Server(5001)
        .OnMessage(data => {
            var message = Encoding.UTF8.GetString(data);
            Console.WriteLine($"[Server] Received: {message}");
            return Encoding.UTF8.GetBytes($"Echo: {message}");
        })
        .Start();
        
    await Task.Delay(Timeout.Infinite);
});

// Give server time to start
await Task.Delay(1000);

// Create client and send messages
using var client = Secure.Client("localhost", 5001);

for (int i = 0; i < 5; i++)
{
    var response = await client.SendAsync($"Message {i}");
    Console.WriteLine($"[Client] Response: {response}");
    await Task.Delay(500);
}

Console.WriteLine("Demo complete!");
```

Run it:

```bash
dotnet run
```

You should see encrypted messages being exchanged between client and server!