using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class ErrorMessage : Message
    {
        // we will need to determine error numbers for possible errors
        private int errorNumber;
        
        public ErrorMessage() { this.messageType = 12; }
        public ErrorMessage(int errorNum, Identifier messageNum, Identifier convoId)
        {
            this.messageType = 12;
            this.errorNumber = errorNum;
            this.MessageNumber = messageNum;
            this.ConversationId = convoId;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeInt(memoryStream, this.errorNumber);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(int errorCode, Identifier messageNum, Identifier convoId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeInt(memoryStream, errorCode);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, convoId);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.errorNumber = decodeInt(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }
        public int getErrorNumber() { return this.errorNumber; }
    }
}
