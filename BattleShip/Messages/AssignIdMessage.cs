using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class AssignIdMessage : Message
    {
        private short Id;
        private byte[] sysKey;
        private byte[] sysIV;

        public AssignIdMessage() { this.messageType = 3;  }
        public AssignIdMessage(short Id, byte[] encryptedKey, byte[] encryptedIV, Identifier num, Identifier ConId)
        {
            this.messageType = 3;
            this.MessageNumber = num;
            this.ConversationId = ConId;
            this.Id = Id;
            this.sysKey = encryptedKey;
            this.sysIV = encryptedIV;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.Id);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            encodeInt(memoryStream, this.sysKey.Length);
            memoryStream.Write(this.sysKey, 0, this.sysKey.Length);
            encodeInt(memoryStream, this.sysIV.Length);
            memoryStream.Write(this.sysIV, 0, this.sysIV.Length);
            return memoryStream.ToArray();
        }
        public byte[] encode(short Id, byte[] sysKey, byte[] sysIV, Identifier num, Identifier ConId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, Id);
            encodeIdentifier(memoryStream, num);
            encodeIdentifier(memoryStream, ConId);
            encodeInt(memoryStream, sysKey.Length);
            memoryStream.Write(sysKey, 0, sysKey.Length);
            encodeInt(memoryStream, sysIV.Length);
            memoryStream.Write(sysIV, 0, sysIV.Length);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.Id = decodeShort(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
            int keyLength = decodeInt(memoryStream);
            this.sysKey = new byte[keyLength];
            memoryStream.Read(this.sysKey, 0, keyLength);
            int ivLength = decodeInt(memoryStream);
            this.sysIV = new byte[ivLength];
            memoryStream.Read(this.sysIV, 0, ivLength);
        }

        public short getId() { return this.Id; }
        public byte[] getKey() { return this.sysKey; }
        public byte[] getIV() { return this.sysIV; }
    }
}
