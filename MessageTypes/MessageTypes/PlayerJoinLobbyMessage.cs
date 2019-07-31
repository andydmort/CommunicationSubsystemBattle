using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class PlayerJoinLobbyMessage : Message
    {
        private string playerName;
        public PlayerJoinLobbyMessage() { this.messageType = 2; }
        public PlayerJoinLobbyMessage(string playerName)
        {
            this.messageType = 2;
            this.playerName = playerName;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeString(memoryStream, this.playerName);
            return memoryStream.ToArray();
        }
        public byte[] encode(string playerName)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeString(memoryStream, playerName);
            return memoryStream.ToArray();
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.playerName = decodeString(memoryStream);
        }

        public string getPlayerName() { return this.playerName; }
    }
}
