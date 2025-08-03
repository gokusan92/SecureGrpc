# SecureGrpc - Rider Setup Guide

## Opening in JetBrains Rider

1. **Open the Solution**
   - In Rider: File → Open → Select `SecureGrpc.sln`
   - The solution should load with 3 projects:
     - `src/SecureGrpc` - Main library
     - `tests/SecureGrpc.Tests` - Unit tests  
     - `Example` - Usage examples

2. **Build the Solution**
   - Build → Build Solution (Ctrl+F9)
   - Or in terminal: `dotnet build`

3. **Run Tests**
   - Right-click on `SecureGrpc.Tests` → Run Unit Tests
   - Or in terminal: `dotnet test`

4. **Run Examples**
   - Right-click on `Example` project → Run
   - Or in terminal: `dotnet run --project Example/Example.csproj`

## Using SecureGrpc in Your Project

### 1. Add NuGet Package (when published)
```xml
<PackageReference Include="SecureGrpc" Version="1.0.0" />
```

### 2. Simple Usage
```csharp
using SecureGrpc;

// Server
using var server = Secure.Server(5001)
    .OnMessage(data => ProcessData(data))
    .Start();

// Client  
using var client = Secure.Client("localhost", 5001);
await client.SendAsync("Hello!");
```

### 3. With Existing gRPC Services
```csharp
using SecureGrpc.Extensions;

// Add encryption to channel
var secureInvoker = channel.WithEncryption();
var client = new YourService.YourServiceClient(secureInvoker);

// Add encryption to service
server.Services.Add(service.WithEncryption());
```

## Project Structure in Rider

```
SecureGrpc.sln
├── 📁 src
│   └── 📦 SecureGrpc (Class Library)
│       ├── SecureGrpc.cs
│       ├── Client/
│       ├── Server/
│       ├── Crypto/
│       ├── Protocol/
│       └── Extensions/
├── 📁 tests  
│   └── 🧪 SecureGrpc.Tests (xUnit)
│       └── IntegrationTests.cs
└── 📁 examples
    └── 🚀 Example (Console App)
        ├── Program.cs
        └── WithEncryptionExample.cs
```

## Troubleshooting

1. **Solution won't load**: Make sure you open `SecureGrpc.sln`, not individual project files
2. **Build errors**: Run `dotnet restore` first
3. **Tests fail**: Make sure port 50123 is available (used by integration tests)

## Publishing to NuGet

1. Update version in `src/SecureGrpc/SecureGrpc.csproj`
2. Build in Release mode: `dotnet build -c Release`
3. Create package: `dotnet pack -c Release`
4. Push to NuGet: `dotnet nuget push bin/Release/*.nupkg -s https://api.nuget.org/v3/index.json`