# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability in SecureGrpc, please report it to us through:

1. **GitHub Security Advisories**: Use the "Security" tab in this repository
2. **Email**: Create an issue describing the vulnerability (without sensitive details)

Please include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

We will respond within 48 hours and work on a fix as soon as possible.

## Security Features

SecureGrpc implements:
- Post-quantum secure encryption using ML-KEM (Kyber) and Diffie-Hellman
- AES-256-GCM for symmetric encryption
- Certificate-based authentication
- Protection against downgrade attacks

## Dependencies

We regularly update dependencies to patch security vulnerabilities. The library has migrated from the deprecated and vulnerable Grpc.Core to the secure Grpc.Net.Client.