# Add GitHub Actions Workflows

To enable the build badge, create these files on GitHub:

## 1. Create `.github/workflows/build.yml`:

```yaml
name: Build and Test

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore -c Release
      
    - name: Test
      run: dotnet test --no-build --verbosity normal -c Release
```

## 2. Create `.github/workflows/publish.yml`:

```yaml
name: Publish to NuGet

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
        
    - name: Build
      run: dotnet build src/SecureGrpc/SecureGrpc.csproj -c Release
      
    - name: Pack
      run: dotnet pack src/SecureGrpc/SecureGrpc.csproj -c Release
      
    - name: Publish to NuGet
      run: dotnet nuget push src/SecureGrpc/bin/Release/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{secrets.NUGET_API_KEY}} --skip-duplicate
```

## Steps:
1. Go to https://github.com/gokusan92/SecureGrpc
2. Click "Create new file"
3. Name it `.github/workflows/build.yml`
4. Paste the build workflow content
5. Commit
6. Repeat for `publish.yml`

Then your build badge will work!