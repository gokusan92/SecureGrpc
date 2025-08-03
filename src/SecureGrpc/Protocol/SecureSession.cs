using System.Security.Cryptography;

namespace SecureGrpc.Protocol;

/// <summary>
/// Represents a secure session between client and server
/// </summary>
internal class SecureSession
{
    public string Id { get; set; }
    public byte[] SharedSecret { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public SecureSession(string id, byte[] sharedSecret)
    {
        Id = id;
        SharedSecret = sharedSecret;
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Derive a key for specific purpose using HKDF
    /// </summary>
    public byte[] DeriveKey(string purpose = "encryption")
    {
        using var hmac = new HMACSHA256(SharedSecret);
        return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes($"{Id}:{purpose}"));
    }
    
    /// <summary>
    /// Check if session is still valid (24 hour timeout)
    /// </summary>
    public bool IsValid => (DateTime.UtcNow - CreatedAt).TotalHours < 24;
}