using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class GMJoinLobbyMessage : Message
    {
        public GMJoinLobbyMessage(){ this.messageType = 1; }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            // does not have a decode method because all this class is is a messageType
            // messageType decoding will be handled in base message class
        }
    }
}
