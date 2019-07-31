using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class EndGameResultMessage : Message
    {
        private short playerId;
        private short gameId;

        public EndGameResultMessage() { this.messageType = 11; }
        public EndGameResultMessage(short playerId, short gameId, Identifier messageNum, Identifier convoId)
        {
            this.messageType = 11;
            this.MessageNumber = messageNum;
            this.ConversationId = convoId;
            this.playerId = playerId;
            this.gameId = gameId;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.gameId);
            encodeShort(memoryStream, this.playerId);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short playerId, short gameId, Identifier messageNum, Identifier convoId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, gameId);
            encodeShort(memoryStream, playerId);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, convoId);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.gameId = decodeShort(memoryStream);
            this.playerId = decodeShort(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public short getGameId() { return this.gameId; }
        public short getplayerId() { return this.playerId; }
    }
}
