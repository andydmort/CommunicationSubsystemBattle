using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class ResultMessage : Message
    {
        private short hit = 0;
        private short gameId;
        private short win = 0;
        private short end = 0;
        private short myTurn = 0;
        private short Xcord;
        private short Ycord;

        public ResultMessage() { this.messageType = 10; }
        public ResultMessage(bool hit, short gameId, bool win, bool end, bool myTurn, short X, short Y, Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 10;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            if (hit) this.hit = 1;
            this.gameId = gameId;
            if (win) this.win = 1;
            if (end) this.end = 1;
            if (myTurn) this.myTurn = 1;
            this.Xcord = X;
            this.Ycord = Y;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.hit);
            encodeShort(memoryStream, this.gameId);
            encodeShort(memoryStream, this.win);
            encodeShort(memoryStream, this.end);
            encodeShort(memoryStream, this.myTurn);
            encodeShort(memoryStream, this.Xcord);
            encodeShort(memoryStream, this.Ycord);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(bool hit, short gameId, bool win, bool end, bool myTurn, short X, short Y, Identifier messageNum, Identifier conversationId)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, messageType);
            if (hit) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            encodeShort(memoryStream, gameId);
            if (win) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            if (end) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            if (myTurn) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            encodeShort(memoryStream, Xcord);
            encodeShort(memoryStream, Ycord);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.hit = decodeShort(memoryStream);
            this.gameId = decodeShort(memoryStream);
            this.win = decodeShort(memoryStream);
            this.end = decodeShort(memoryStream);
            this.myTurn = decodeShort(memoryStream);
            this.Xcord = decodeShort(memoryStream);
            this.Ycord = decodeShort(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public bool getHit() {
            if (this.hit == 1) return true;
            else return false;
        }
        public short getGameId() { return this.gameId; }
        public bool getWin() {
            if (this.win == 1) return true;
            return false;
        }
        public bool getEnd() {
            if (this.end == 1) return true;
            return false;
        }
        public bool getMyTurn() {
            if (this.myTurn == 1) return true;
            return false;
        }
        public short getXcord()
        {
            return this.Xcord;
        }
        public short getYcord()
        {
            return this.Ycord;
        }
    }
}
