# SecureGrpc 🔐

[![NuGet](https://img.shields.io/nuget/v/SecureGrpc.svg)](https://www.nuget.org/packages/SecureGrpc/)
[![Build Status](https://github.com/yourusername/SecureGrpc/workflows/Build%20and%20Test/badge.svg)](https://github.com/yourusername/SecureGrpc/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

> ⚠️ **SECURITY WARNING**: This library currently uses `Grpc.Core` which has known vulnerabilities (CVE-2023-32731, CVE-2023-33953). For production use, consider migrating to `Grpc.Net.Client`. See [SECURITY_UPDATE.md](SECURITY_UPDATE.md) for details.

**Post-quantum secure gRPC communication made ridiculously easy!**

SecureGrpc provides transparent end-to-end encryption for gRPC using state-of-the-art cryptography:
- 🛡️ **ML-KEM (Kyber-768)** - Post-quantum secure key encapsulation
- 🔑 **Diffie-Hellman** - Classic perfect forward secrecy
- 🔒 **AES-256-GCM** - Authenticated encryption

## Installation

```bash
dotnet add package SecureGrpc
```

## Quick Start

### Server
```csharp
using SecureGrpc;

// One line to create a secure server!
using var server = Secure.Server(5001)
    .OnMessage(data => {
        Console.WriteLine($"Received: {Encoding.UTF8.GetString(data)}");
        return Encoding.UTF8.GetBytes("Hello from server!");
    })
    .Start();
```

### Client
```csharp
using SecureGrpc;

// One line to create a secure client!
using var client = Secure.Client("localhost", 5001);

// Send messages - automatically encrypted!
var response = await client.SendAsync("Hello server!");
Console.WriteLine($"Server said: {response}");
```

## Middleware Integration

### Add encryption to existing gRPC services

```csharp
// Server-side
var server = new Server()
    .WithEncryption()  // Add this line!
    .Services.Add(YourService.BindService(new YourServiceImpl()));

// Client-side  
var channel = new Channel("localhost", 5001, ChannelCredentials.Insecure)
    .WithEncryption();  // Add this line!
var client = new YourService.YourServiceClient(channel);
```

### Fluent API

```csharp
var channel = "localhost".CreateSecureChannel(5001)
    .WithCredentials(ChannelCredentials.Insecure)
    .Build();
```

## Features

✅ **Zero Configuration** - Works out of the box  
✅ **Post-Quantum Secure** - Resistant to quantum computer attacks  
✅ **Perfect Forward Secrecy** - Past sessions remain secure  
✅ **Automatic Key Management** - No manual key handling  
✅ **Session Management** - Automatic session creation and reuse  
✅ **Cross-Language Compatible** - Implement the protocol in any language  

## How It Works

1. **Automatic Key Exchange**: Client and server automatically perform a hybrid key exchange using both ML-KEM and Diffie-Hellman
2. **Session Establishment**: A secure session is created with a unique shared secret
3. **Transparent Encryption**: All messages are automatically encrypted with AES-256-GCM
4. **Zero Trust**: Each session uses unique keys derived from the shared secret

## Performance

- **Key Exchange**: ~50ms (one-time per session)
- **Encryption/Decryption**: <1ms per message
- **Memory Overhead**: ~10KB per session

## Security Details

### Cryptographic Algorithms
- **Key Exchange**: ML-KEM-768 (Kyber) + DH-2048
- **Encryption**: AES-256-GCM with 128-bit tags
- **Key Derivation**: HMAC-SHA256
- **Random**: Cryptographically secure RNG

### Threat Model
SecureGrpc protects against:
- 🔍 Eavesdropping (including by quantum computers)
- 🔄 Man-in-the-middle attacks (with proper certificate validation)
- 📝 Message tampering
- 🔙 Replay attacks

## Advanced Usage

### Custom Message Handlers
```csharp
var server = Secure.Server(5001)
    .OnMessage(async data => {
        // Async processing
        await ProcessDataAsync(data);
        return responseData;
    })
    .Start();
```

### Multiple Clients
```csharp
var client1 = Secure.Client("server1", 5001);
var client2 = Secure.Client("server2", 5002);

// Each client maintains its own secure session
await Task.WhenAll(
    client1.SendAsync("Hello server 1"),
    client2.SendAsync("Hello server 2")
);
```

## Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Acknowledgments

- [BouncyCastle](https://www.bouncycastle.org/) for cryptographic implementations
- [gRPC](https://grpc.io/) for the RPC framework
- NIST for standardizing ML-KEM

---

**Made with ❤️ for developers who care about security**