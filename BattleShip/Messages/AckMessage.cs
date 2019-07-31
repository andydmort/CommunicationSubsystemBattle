using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class AckMessage : Message
    {
        private short id;
        private byte gORp;

        public AckMessage() { this.messageType = 5; }
        public AckMessage(short id, byte gORp, Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 5;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            this.id = id;
            this.gORp = gORp;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.id);
            memoryStream.WriteByte(this.gORp);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short id, byte gORp, Identifier messageNum, Identifier conversationId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, id);
            memoryStream.WriteByte(gORp);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.id = decodeShort(memoryStream);
            this.gORp = Convert.ToByte(memoryStream.ReadByte());
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public short getId() { return this.id; }
        public byte getGORp() { return this.gORp; }
    }
}
