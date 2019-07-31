using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public class GameIdMessage : Message
    {
        private short gameId;
        private short p1Id;
        private short p2Id;
        private string GMEndpoint;
        private string p1Name;
        private string p2Name;
        private bool p1Win;
        private bool p2Win;
        private bool p1Lose;
        private bool p2Lose;

        public GameIdMessage() { this.messageType = 7; }
        public GameIdMessage(short gameId, short p1Id, short p2Id, string GMEndpoint, string p1Name,
                               string p2Name, bool p1Win, bool p2Win, bool p1Lose, bool p2Lose)
        {
            this.messageType = 7;
            this.gameId = gameId;
            this.p1Id = p1Id;
            this.p2Id = p2Id;
            this.GMEndpoint = GMEndpoint;
            this.p1Name = p1Name;
            this.p2Name = p2Name;
            this.p1Win = p1Win;
            this.p2Win = p2Win;
            this.p1Lose = p1Lose;
            this.p2Lose = p2Lose;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.gameId);
            encodeShort(memoryStream, this.p1Id);
            encodeShort(memoryStream, this.p2Id);
            encodeString(memoryStream, this.GMEndpoint);
            encodeString(memoryStream, this.p1Name);
            encodeString(memoryStream, this.p2Name);
            encodeBool(memoryStream, this.p1Win);
            encodeBool(memoryStream, this.p2Win);
            encodeBool(memoryStream, this.p1Lose);
            encodeBool(memoryStream, this.p2Lose);
            return memoryStream.ToArray();
        }
        public byte[] encode(short gameId, short p1Id, short p2Id, string GMEndpoint, string p1Name,
                             string p2Name, bool p1Win, bool p2Win, bool p1lose, bool p2lose)

        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, gameId);
            encodeShort(memoryStream, p1Id);
            encodeShort(memoryStream, p2Id);
            encodeString(memoryStream, GMEndpoint);
            encodeString(memoryStream, p1Name);
            encodeString(memoryStream, p2Name);
            encodeBool(memoryStream, p1Win);
            encodeBool(memoryStream, p2Win);
            encodeBool(memoryStream, p1Lose);
            encodeBool(memoryStream, p2Lose);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            // messages should not have their message type at this point
            // message types should be stripped off of the memory stream at the dispatcher
            var memoryStream = new MemoryStream(bytes);
            this.gameId = decodeShort(memoryStream);
            this.p1Id = decodeShort(memoryStream);
            this.p2Id = decodeShort(memoryStream);
            this.GMEndpoint = decodeString(memoryStream);
            this.p1Name = decodeString(memoryStream);
            this.p2Name = decodeString(memoryStream);
            this.p1Win = decodeBool(memoryStream);
            this.p2Win = decodeBool(memoryStream);
            this.p1Lose = decodeBool(memoryStream);
            this.p2Lose = decodeBool(memoryStream);
        }

        public short getGameId() { return this.gameId; }
        public short getP1Id() { return this.p1Id; }
        public short getP2Id() { return this.p2Id; }
        public string getGMEndpoint() { return this.GMEndpoint; }
        public string getP1Name() { return this.p1Name; }
        public string getP2Name() { return this.p2Name; }
        public bool getP1Win() { return this.p1Win; }
        public bool getP2Win() { return this.p2Win; }
        public bool getP1Lose() { return this.p1Lose; }
        public bool getP2Lose() { return this.p2Lose; }
    }
}
