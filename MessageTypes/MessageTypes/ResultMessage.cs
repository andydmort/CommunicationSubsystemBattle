using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class ResultMessage : Message
    {
        private bool hit;
        private short gameId;
        private bool win;
        private bool end;

        public ResultMessage() { this.messageType = 10; }
        public ResultMessage(bool hit, short gameId, bool win, bool end)
        {
            this.messageType = 10;
            this.hit = hit;
            this.gameId = gameId;
            this.win = win;
            this.end = end;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeBool(memoryStream, this.hit);
            encodeShort(memoryStream, this.gameId);
            encodeBool(memoryStream, this.win);
            encodeBool(memoryStream, this.end);
            return memoryStream.ToArray();
        }
        public byte[] encode(bool hit, short gameId, bool win, bool end)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, messageType);
            encodeBool(memoryStream, hit);
            encodeShort(memoryStream, gameId);
            encodeBool(memoryStream, win);
            encodeBool(memoryStream, end);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.hit = decodeBool(memoryStream);
            this.gameId = decodeShort(memoryStream);
            this.win = decodeBool(memoryStream);
            this.end = decodeBool(memoryStream);
        }

        public bool getHit() { return this.hit; }
        public short getGameId() { return this.gameId; }
        public bool getWin() { return this.win; }
        public bool getEnd() { return this.end; }
    }
}
