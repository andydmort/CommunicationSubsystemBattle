using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class HeartbeatMessage : Message
    {
        private short ID;
        private byte gORp;

        public HeartbeatMessage() { this.messageType = 4; }
        public HeartbeatMessage(short ID, byte gORp, Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 4;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            this.ID = ID;
            this.gORp = gORp;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.ID);
            memoryStream.WriteByte(this.gORp);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short ID, byte gORp, Identifier messageNum, Identifier conversationId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, ID);
            memoryStream.WriteByte(gORp);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.ID = decodeShort(memoryStream);
            this.gORp = Convert.ToByte(memoryStream.ReadByte());
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public short getID() { return this.ID; }
        public int getGORp() { return (int)this.gORp; }
    }
}
