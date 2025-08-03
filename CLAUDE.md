# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 console application project named "encsample".

## Build and Run Commands

### Build the project
```bash
dotnet build
```

### Run the application
```bash
dotnet run
```

### Build in Release mode
```bash
dotnet build -c Release
```

### Clean build artifacts
```bash
dotnet clean
```

### Run tests (when tests are added)
```bash
dotnet test
```

### Run a specific test (when tests are added)
```bash
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
```

## Project Structure

- `encsample.sln` - Solution file at the parent directory level
- `encsample/` - Main project directory
  - `encsample.csproj` - Project file targeting .NET 9.0 with nullable reference types enabled
  - `Program.cs` - Application entry point

## Development Notes

- The project uses implicit usings and nullable reference types are enabled
- Output type is configured as a console executable
- Implements post-quantum secure gRPC middleware with ML-KEM and Diffie-Hellman
- Uses BouncyCastle for cryptographic operations
- Custom TLS 1.3-like handshake protocol with certificate authority

## Architecture

### Core Components

- **Crypto/Algos/** - Cryptographic primitives (ML-KEM, DH, AES-GCM)
- **Auth/** - Certificate authority and certificate management
- **Middleware/** - gRPC interceptors, handshake protocol, secure channel
- **Proto/** - Protobuf message definitions
- **Services/** - Example gRPC services

### Key Classes

- `SecureGrpc` - Main API entry point for creating secure clients/servers
- `Handshake` - Manages TLS 1.3-like handshake with hybrid PQ crypto
- `SecureInterceptor` - gRPC interceptor for transparent encryption
- `CA` - Simple certificate authority for issuing/verifying certificates