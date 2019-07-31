using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;  

namespace Messages
{
    public class PlayerJoinLobbyMessage : Message
    {
        private string playerName;
        private string publicKey;
        public PlayerJoinLobbyMessage() { this.messageType = 2; }
        public PlayerJoinLobbyMessage(string playerName, string publicKey, Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 2;
            this.playerName = playerName;
            this.publicKey = publicKey;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeString(memoryStream, this.playerName);
            encodeString(memoryStream, publicKey);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(string playerName, string publicKey, Identifier messageNum, Identifier conversationId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeString(memoryStream, playerName);
            encodeString(memoryStream, publicKey);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.playerName = decodeString(memoryStream);
            this.publicKey = decodeString(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public string getPlayerName() { return this.playerName; }
        public string getPublicKey() { return this.publicKey; }
    }
}
