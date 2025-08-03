using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace SecureGrpc.Crypto;

/// <summary>
/// Hybrid post-quantum cryptography combining ML-KEM and Diffie-Hellman
/// </summary>
internal class HybridCrypto
{
    private readonly SecureRandom _random = new();
    private readonly DHParameters _dhParams;
    
    public HybridCrypto()
    {
        // Pre-computed 2048-bit MODP Group parameters (RFC 3526)
        var p = new Org.BouncyCastle.Math.BigInteger(
            "FFFFFFFFFFFFFFFFC90FDAA22168C234C4C6628B80DC1CD129024E088A67CC74" +
            "020BBEA63B139B22514A08798E3404DDEF9519B3CD3A431B302B0A6DF25F1437" +
            "4FE1356D6D51C245E485B576625E7EC6F44C42E9A637ED6B0BFF5CB6F406B7ED" +
            "EE386BFB5A899FA5AE9F24117C4B1FE649286651ECE45B3DC2007CB8A163BF05" +
            "98DA48361C55D39A69163FA8FD24CF5F83655D23DCA3AD961C62F356208552BB" +
            "9ED529077096966D670C354E4ABC9804F1746C08CA18217C32905E462E36CE3B" +
            "E39E772C180E86039B2783A2EC07A28FB5C55DF06F4C52C9DE2BCBF695581718" +
            "3995497CEA956AE515D2261898FA051015728E5A8AACAA68FFFFFFFFFFFFFFFF", 16);
        var g = new Org.BouncyCastle.Math.BigInteger("2");
        _dhParams = new DHParameters(p, g);
    }
    
    /// <summary>
    /// Generate both DH and ML-KEM key pairs
    /// </summary>
    public (byte[] dhPub, byte[] mlkemPub, AsymmetricKeyParameter dhPriv, byte[] mlkemPriv) GenerateKeyPairs()
    {
        // Diffie-Hellman
        var dhKeyGen = new DHKeyPairGenerator();
        dhKeyGen.Init(new DHKeyGenerationParameters(_random, _dhParams));
        var dhKeyPair = dhKeyGen.GenerateKeyPair();
        var dhPub = ((DHPublicKeyParameters)dhKeyPair.Public).Y.ToByteArrayUnsigned();
        
        // ML-KEM (Kyber)
        var mlkemKeyGen = new KyberKeyPairGenerator();
        mlkemKeyGen.Init(new KyberKeyGenerationParameters(_random, KyberParameters.kyber768));
        var mlkemKeyPair = mlkemKeyGen.GenerateKeyPair();
        var mlkemPub = ((KyberPublicKeyParameters)mlkemKeyPair.Public).GetEncoded();
        var mlkemPriv = ((KyberPrivateKeyParameters)mlkemKeyPair.Private).GetEncoded();
        
        return (dhPub, mlkemPub, dhKeyPair.Private, mlkemPriv);
    }
    
    /// <summary>
    /// Encapsulate ML-KEM and compute hybrid shared secret
    /// </summary>
    public (byte[] ciphertext, byte[] sharedSecret) EncapsulateAndCompute(
        byte[] peerDhPub, byte[] peerMlkemPub, AsymmetricKeyParameter dhPriv)
    {
        // ML-KEM encapsulation
        var mlkemPubKey = new KyberPublicKeyParameters(KyberParameters.kyber768, peerMlkemPub);
        var kemGen = new KyberKemGenerator(_random);
        var encapsulated = kemGen.GenerateEncapsulated(mlkemPubKey);
        
        // DH agreement
        var dhPubKey = new DHPublicKeyParameters(
            new Org.BouncyCastle.Math.BigInteger(1, peerDhPub), _dhParams);
        var dhAgreement = new DHBasicAgreement();
        dhAgreement.Init(dhPriv);
        var dhShared = dhAgreement.CalculateAgreement(dhPubKey).ToByteArrayUnsigned();
        
        // Combine secrets
        var combined = CombineSecrets(dhShared, encapsulated.GetSecret());
        
        return (encapsulated.GetEncapsulation(), DeriveKey(combined));
    }
    
    /// <summary>
    /// Compute hybrid shared secret from received values
    /// </summary>
    public byte[] ComputeSharedSecret(AsymmetricKeyParameter dhPriv, byte[] peerDhPub,
        byte[] mlkemPriv, byte[] mlkemCiphertext)
    {
        // DH agreement
        var dhPubKey = new DHPublicKeyParameters(
            new Org.BouncyCastle.Math.BigInteger(1, peerDhPub), _dhParams);
        var dhAgreement = new DHBasicAgreement();
        dhAgreement.Init(dhPriv);
        var dhShared = dhAgreement.CalculateAgreement(dhPubKey).ToByteArrayUnsigned();
        
        // ML-KEM decapsulation
        var mlkemPrivKey = new KyberPrivateKeyParameters(KyberParameters.kyber768, mlkemPriv);
        var kemExt = new KyberKemExtractor(mlkemPrivKey);
        var mlkemShared = kemExt.ExtractSecret(mlkemCiphertext);
        
        // Combine secrets
        var combined = CombineSecrets(dhShared, mlkemShared);
        
        return DeriveKey(combined);
    }
    
    private byte[] CombineSecrets(byte[] dhShared, byte[] mlkemShared)
    {
        var combined = new byte[dhShared.Length + mlkemShared.Length];
        Buffer.BlockCopy(dhShared, 0, combined, 0, dhShared.Length);
        Buffer.BlockCopy(mlkemShared, 0, combined, dhShared.Length, mlkemShared.Length);
        return combined;
    }
    
    private byte[] DeriveKey(byte[] combinedSecret)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(combinedSecret);
    }
}