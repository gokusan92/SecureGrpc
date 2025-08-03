#!/bin/bash
# Script to publish SecureGrpc to NuGet.org

# Build in Release mode
echo "Building SecureGrpc in Release mode..."
cd src/SecureGrpc
dotnet build -c Release

# Create NuGet package
echo "Creating NuGet package..."
dotnet pack -c Release

# Show package info
echo "Package created:"
ls -la bin/Release/*.nupkg

echo ""
echo "To publish to NuGet.org, run:"
echo "dotnet nuget push bin/Release/SecureGrpc.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_NUGET_API_KEY"
echo ""
echo "Replace YOUR_NUGET_API_KEY with your actual NuGet API key from https://www.nuget.org/account/apikeys"