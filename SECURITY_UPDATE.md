# Security Update Required

## Vulnerabilities Detected

- **GHSA-6628-q6j9-w8vg** (Score: 7.5) - High severity
- **GHSA-9hxf-ppjv-w6rq** (Score: 5.3) - Medium severity

## Issue

The `Grpc.Core` package (version 2.46.6) has known vulnerabilities and is deprecated by Google.

## Solution

To fix these vulnerabilities, you need to migrate from `Grpc.Core` to `Grpc.Net.Client`:

### 1. Update SecureGrpc.csproj

Replace:
```xml
<PackageReference Include="Grpc.Core" Version="2.46.6" />
```

With:
```xml
<PackageReference Include="Grpc.Net.Client" Version="2.65.0" />
<PackageReference Include="Grpc.AspNetCore" Version="2.65.0" />
```

### 2. Code Changes Required

The migration requires updating:
- `Channel` → `GrpcChannel`
- `Server` → ASP.NET Core hosting model
- `CallInvoker` → `GrpcChannel.CreateCallInvoker()`

### 3. Benefits

- Removes security vulnerabilities
- Better performance
- Native HTTP/2 support
- Cross-platform compatibility

## Alternative

If you need to stay with Grpc.Core for compatibility:
- Be aware of the security risks
- Implement additional security measures
- Plan migration to Grpc.Net.Client

## Recommendation

**Strongly recommend migrating to Grpc.Net.Client** to eliminate these vulnerabilities.