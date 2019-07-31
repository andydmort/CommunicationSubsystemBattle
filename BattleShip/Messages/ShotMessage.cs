using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class ShotMessage : Message
    {
        private short XCord;
        private short YCord;
        private short gameId;
        private short playerId;

        public ShotMessage() { this.messageType = 9; }
        public ShotMessage(short XCord, short YCord, short gameId, short playerId, Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 9;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            this.XCord = XCord;
            this.YCord = YCord;
            this.gameId = gameId;
            this.playerId = playerId;
        }

        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.XCord);
            encodeShort(memoryStream, this.YCord);
            encodeShort(memoryStream, this.gameId);
            encodeShort(memoryStream, this.playerId);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short XCord, short YCord, short gameId, short playerId, Identifier messageNum, Identifier conversationId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, XCord);
            encodeShort(memoryStream, YCord);
            encodeShort(memoryStream, gameId);
            encodeShort(memoryStream, playerId);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }

        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.XCord = decodeShort(memoryStream);
            this.YCord = decodeShort(memoryStream);
            this.gameId = decodeShort(memoryStream);
            this.playerId = decodeShort(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public short getXcord() { return this.XCord; }
        public short getYcord() { return this.YCord; }
        public short getGameId() { return this.gameId; }
        public short getPlayerId() { return this.playerId; }
    }
}
