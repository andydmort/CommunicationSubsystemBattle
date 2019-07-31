using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class AssignIdMessage : Message
    {
        private short Id;
        public AssignIdMessage() { this.messageType = 3;  }
        public AssignIdMessage(short Id)
        {
            this.messageType = 3;
            this.Id = Id;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.Id);
            return memoryStream.ToArray();
        }
        public byte[] encode(short Id)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, Id);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.Id = decodeShort(memoryStream);
        }

        public short getId() { return this.Id; }
    }
}
