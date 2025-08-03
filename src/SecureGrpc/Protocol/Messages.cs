using Google.Protobuf;

namespace SecureGrpc.Protocol;

/// <summary>
/// Protocol message definitions
/// </summary>
internal static class Messages
{
    internal class KeyExchangeRequest : IMessage<KeyExchangeRequest>
    {
        public string ClientId { get; set; } = "";
        public ByteString DhPublicKey { get; set; } = ByteString.Empty;
        public ByteString MlkemPublicKey { get; set; } = ByteString.Empty;
        
        public void WriteTo(CodedOutputStream output)
        {
            if (!string.IsNullOrEmpty(ClientId))
            {
                output.WriteTag(1, WireFormat.WireType.LengthDelimited);
                output.WriteString(ClientId);
            }
            if (DhPublicKey.Length > 0)
            {
                output.WriteTag(2, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(DhPublicKey);
            }
            if (MlkemPublicKey.Length > 0)
            {
                output.WriteTag(3, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(MlkemPublicKey);
            }
        }
        
        public int CalculateSize()
        {
            var size = 0;
            if (!string.IsNullOrEmpty(ClientId))
                size += 1 + CodedOutputStream.ComputeStringSize(ClientId);
            if (DhPublicKey.Length > 0)
                size += 1 + CodedOutputStream.ComputeBytesSize(DhPublicKey);
            if (MlkemPublicKey.Length > 0)
                size += 1 + CodedOutputStream.ComputeBytesSize(MlkemPublicKey);
            return size;
        }
        
        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag >> 3)
                {
                    case 1: ClientId = input.ReadString(); break;
                    case 2: DhPublicKey = input.ReadBytes(); break;
                    case 3: MlkemPublicKey = input.ReadBytes(); break;
                    default: input.SkipLastField(); break;
                }
            }
        }
        
        public void MergeFrom(KeyExchangeRequest message) { }
        public Google.Protobuf.Reflection.MessageDescriptor Descriptor => null!;
        public KeyExchangeRequest Clone() => new() 
        { 
            ClientId = ClientId, 
            DhPublicKey = DhPublicKey, 
            MlkemPublicKey = MlkemPublicKey 
        };
        public bool Equals(KeyExchangeRequest? other) => other?.ClientId == ClientId;
        
        public static MessageParser<KeyExchangeRequest> Parser { get; } = 
            new MessageParser<KeyExchangeRequest>(() => new KeyExchangeRequest());
        
        public byte[] ToByteArray()
        {
            var size = CalculateSize();
            var buffer = new byte[size];
            var output = new CodedOutputStream(buffer);
            WriteTo(output);
            output.Flush();
            return buffer;
        }
    }
    
    internal class KeyExchangeReply : IMessage<KeyExchangeReply>
    {
        public string SessionId { get; set; } = "";
        public ByteString DhPublicKey { get; set; } = ByteString.Empty;
        public ByteString MlkemCiphertext { get; set; } = ByteString.Empty;
        
        public void WriteTo(CodedOutputStream output)
        {
            if (!string.IsNullOrEmpty(SessionId))
            {
                output.WriteTag(1, WireFormat.WireType.LengthDelimited);
                output.WriteString(SessionId);
            }
            if (DhPublicKey.Length > 0)
            {
                output.WriteTag(2, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(DhPublicKey);
            }
            if (MlkemCiphertext.Length > 0)
            {
                output.WriteTag(3, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(MlkemCiphertext);
            }
        }
        
        public int CalculateSize()
        {
            var size = 0;
            if (!string.IsNullOrEmpty(SessionId))
                size += 1 + CodedOutputStream.ComputeStringSize(SessionId);
            if (DhPublicKey.Length > 0)
                size += 1 + CodedOutputStream.ComputeBytesSize(DhPublicKey);
            if (MlkemCiphertext.Length > 0)
                size += 1 + CodedOutputStream.ComputeBytesSize(MlkemCiphertext);
            return size;
        }
        
        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag >> 3)
                {
                    case 1: SessionId = input.ReadString(); break;
                    case 2: DhPublicKey = input.ReadBytes(); break;
                    case 3: MlkemCiphertext = input.ReadBytes(); break;
                    default: input.SkipLastField(); break;
                }
            }
        }
        
        public void MergeFrom(KeyExchangeReply message) { }
        public Google.Protobuf.Reflection.MessageDescriptor Descriptor => null!;
        public KeyExchangeReply Clone() => new() 
        { 
            SessionId = SessionId, 
            DhPublicKey = DhPublicKey, 
            MlkemCiphertext = MlkemCiphertext 
        };
        public bool Equals(KeyExchangeReply? other) => other?.SessionId == SessionId;
        
        public static MessageParser<KeyExchangeReply> Parser { get; } = 
            new MessageParser<KeyExchangeReply>(() => new KeyExchangeReply());
        
        public byte[] ToByteArray()
        {
            var size = CalculateSize();
            var buffer = new byte[size];
            var output = new CodedOutputStream(buffer);
            WriteTo(output);
            output.Flush();
            return buffer;
        }
    }
    
    internal class SecureMessage : IMessage<SecureMessage>
    {
        public ByteString EncryptedData { get; set; } = ByteString.Empty;
        
        public void WriteTo(CodedOutputStream output)
        {
            if (EncryptedData.Length > 0)
            {
                output.WriteTag(1, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(EncryptedData);
            }
        }
        
        public int CalculateSize()
        {
            return EncryptedData.Length > 0 
                ? 1 + CodedOutputStream.ComputeBytesSize(EncryptedData) 
                : 0;
        }
        
        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                if ((tag >> 3) == 1)
                    EncryptedData = input.ReadBytes();
                else
                    input.SkipLastField();
            }
        }
        
        public void MergeFrom(SecureMessage message) { }
        public Google.Protobuf.Reflection.MessageDescriptor Descriptor => null!;
        public SecureMessage Clone() => new() { EncryptedData = EncryptedData };
        public bool Equals(SecureMessage? other) => other?.EncryptedData.Equals(EncryptedData) ?? false;
        
        public static MessageParser<SecureMessage> Parser { get; } = 
            new MessageParser<SecureMessage>(() => new SecureMessage());
        
        public byte[] ToByteArray()
        {
            var size = CalculateSize();
            var buffer = new byte[size];
            var output = new CodedOutputStream(buffer);
            WriteTo(output);
            output.Flush();
            return buffer;
        }
    }
}