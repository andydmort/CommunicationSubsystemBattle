using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class AckMessage : Message
    {
        private short ID;
        private byte gORp;

        public AckMessage() { this.messageType = 5; }
        public AckMessage(short ID, byte gORp)
        {
            this.messageType = 5;
            this.ID = ID;
            this.gORp = gORp;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.ID);
            memoryStream.WriteByte(this.gORp);
            return memoryStream.ToArray();
        }
        public byte[] encode(short ID, byte gORp)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, ID);
            memoryStream.WriteByte(gORp);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.ID = decodeShort(memoryStream);
            this.gORp = Convert.ToByte(memoryStream.ReadByte());
        }

        public short getID() { return this.ID; }
        public int getGORp() { return (int)this.gORp; }
    }
}
