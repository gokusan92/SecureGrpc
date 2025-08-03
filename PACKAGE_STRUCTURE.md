# SecureGrpc Package Structure

## Library Structure
```
SecureGrpc/
├── src/
│   └── SecureGrpc/
│       ├── SecureGrpc.cs              # Main API entry point
│       ├── Server/
│       │   └── SecureServer.cs        # Server implementation
│       ├── Client/
│       │   └── SecureClient.cs        # Client implementation
│       ├── Crypto/
│       │   ├── HybridCrypto.cs        # ML-KEM + DH hybrid crypto
│       │   └── AesGcm.cs              # AES-256-GCM encryption
│       ├── Protocol/
│       │   ├── Messages.cs            # Protocol messages
│       │   ├── SecureGrpcService.cs   # gRPC service definition
│       │   ├── SecureServiceImpl.cs   # Server implementation
│       │   ├── SecureClientImpl.cs    # Client implementation
│       │   └── SecureSession.cs       # Session management
│       └── Extensions/
│           ├── SecureGrpcExtensions.cs    # WithEncryption() methods
│           ├── SecureClientInterceptor.cs # Client interceptor
│           └── SecureServerInterceptor.cs # Server interceptor
├── tests/
│   └── SecureGrpc.Tests/
│       ├── CryptoTests.cs             # Crypto unit tests
│       └── IntegrationTests.cs        # End-to-end tests
├── Example/
│   ├── Program.cs                     # Usage examples
│   └── WithEncryptionExample.cs       # Middleware examples
└── .github/
    └── workflows/
        ├── build.yml                  # CI build & test
        └── publish.yml                # NuGet publish

```

## Key Features

### 1. Simple API
```csharp
// Server
using var server = Secure.Server(5001)
    .OnMessage(data => ProcessData(data))
    .Start();

// Client
using var client = Secure.Client("localhost", 5001);
await client.SendAsync("Hello!");
```

### 2. WithEncryption() Middleware
```csharp
// Add to any channel
var secureInvoker = channel.WithEncryption();

// Add to any service
server.Services.Add(service.WithEncryption());
```

### 3. Fluent API
```csharp
var client = "localhost"
    .CreateSecureChannel(5001)
    .BuildSecure();
```

## Installation
```bash
dotnet add package SecureGrpc
```

## Security
- ML-KEM-768 (Post-quantum)
- Diffie-Hellman 2048-bit
- AES-256-GCM
- HMAC-SHA256