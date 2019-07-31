using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class GMJoinLobbyMessage : Message
    {
        public int port;
        public string publicKey;
        public GMJoinLobbyMessage(){ this.messageType = 1; }
        public GMJoinLobbyMessage(string publicKey, Identifier messageNum, Identifier conversationId, int port){
            this.messageType = 1;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            this.port = port;
            this.publicKey = publicKey;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            encodeInt(memoryStream, this.port);
            encodeString(memoryStream, publicKey);

            return memoryStream.ToArray();
        }
        public byte[] encode(string publicKey, Identifier messageNum, Identifier conversationId, int port)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, ConversationId);
            encodeInt(memoryStream, port);
            encodeString(memoryStream, publicKey);

            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
            this.port = decodeInt(memoryStream);
            this.publicKey = decodeString(memoryStream);
            
        }
        public int getPort() { return this.port; }
        public string getPublicKey() { return this.publicKey; }
    }
}
