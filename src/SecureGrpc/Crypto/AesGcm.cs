using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SecureGrpc.Crypto;

/// <summary>
/// AES-256-GCM authenticated encryption
/// </summary>
internal class AesGcm
{
    private readonly SecureRandom _random = new();
    private const int NonceSize = 12;
    private const int TagSize = 16;
    
    /// <summary>
    /// Encrypt data with AES-256-GCM
    /// </summary>
    public byte[] Encrypt(byte[] plaintext, byte[] key)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var nonce = new byte[NonceSize];
        _random.NextBytes(nonce);
        
        var parameters = new AeadParameters(new KeyParameter(key), TagSize * 8, nonce);
        cipher.Init(true, parameters);
        
        var ciphertext = new byte[cipher.GetOutputSize(plaintext.Length)];
        var len = cipher.ProcessBytes(plaintext, 0, plaintext.Length, ciphertext, 0);
        cipher.DoFinal(ciphertext, len);
        
        // Prepend nonce to ciphertext
        var result = new byte[nonce.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
        
        return result;
    }
    
    /// <summary>
    /// Decrypt data with AES-256-GCM
    /// </summary>
    public byte[] Decrypt(byte[] encryptedData, byte[] key)
    {
        if (encryptedData.Length < NonceSize + TagSize)
            throw new ArgumentException("Invalid encrypted data");
        
        // Extract nonce
        var nonce = new byte[NonceSize];
        Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
        
        // Extract ciphertext
        var ciphertext = new byte[encryptedData.Length - NonceSize];
        Buffer.BlockCopy(encryptedData, NonceSize, ciphertext, 0, ciphertext.Length);
        
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), TagSize * 8, nonce);
        cipher.Init(false, parameters);
        
        var plaintext = new byte[cipher.GetOutputSize(ciphertext.Length)];
        var len = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, plaintext, 0);
        cipher.DoFinal(plaintext, len);
        
        return plaintext;
    }
}