using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Messages
{
    public class PlayerIdMessage : Message
    {
        private short gameId;
        private short p1Id;
        private short p2Id;
        private string P1EndPoint;
        private string P2EndPoint;
        private string p1Name;
        private string p2Name;
        private short p1Win = 0;
        private short p2Win = 0;
        private short p1Lose = 0;
        private short p2Lose = 0;

        public PlayerIdMessage() { this.messageType = 6; }
        public PlayerIdMessage(short gameId, short p1Id, short p2Id, string P1EndPoint, string P2EndPoint, string p1Name,
                               string p2Name, bool p1Win, bool p2Win, bool p1Lose, bool p2Lose, 
                               Identifier messageNum, Identifier conversationId)
        {
            this.messageType = 6;
            this.MessageNumber = messageNum;
            this.ConversationId = conversationId;
            this.gameId = gameId;
            this.p1Id = p1Id;
            this.p2Id = p2Id;
            this.P1EndPoint = P1EndPoint;
            this.P2EndPoint = P2EndPoint;
            this.p1Name = p1Name;
            this.p2Name = p2Name;
            if (p1Win) this.p1Win = 1;
            if (p2Win) this.p2Win = 1;
            if (p1Lose) this.p1Lose = 1;
            if (p2Lose) this.p2Lose = 1;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.gameId);
            encodeShort(memoryStream, this.p1Id);
            encodeShort(memoryStream, this.p2Id);
            encodeString(memoryStream, this.P1EndPoint);
            encodeString(memoryStream, this.P2EndPoint);
            encodeString(memoryStream, this.p1Name);
            encodeString(memoryStream, this.p2Name);
            encodeShort(memoryStream, this.p1Win);
            encodeShort(memoryStream, this.p2Win);
            encodeShort(memoryStream, this.p1Lose);
            encodeShort(memoryStream, this.p2Lose);
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short gameId, short p1Id, short p2Id, string P1EndPoint, string P2EndPoint, string p1Name,
                             string p2Name, bool p1Win, bool p2Win, bool p1lose, bool p2lose, 
                             Identifier messageNum, Identifier conversationId)

        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, gameId);
            encodeShort(memoryStream, p1Id);
            encodeShort(memoryStream, p2Id);
            encodeString(memoryStream, P1EndPoint);
            encodeString(memoryStream, P2EndPoint);
            encodeString(memoryStream, p1Name);
            encodeString(memoryStream, p2Name);
            if (p1Win) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            if (p2Win) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            if (p1lose) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            if (p2lose) encodeShort(memoryStream, 1);
            else encodeShort(memoryStream, 0);
            encodeIdentifier(memoryStream, messageNum);
            encodeIdentifier(memoryStream, conversationId);
            return memoryStream.ToArray();
        }
           
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.gameId = decodeShort(memoryStream);
            this.p1Id = decodeShort(memoryStream);
            this.p2Id = decodeShort(memoryStream);
            this.P1EndPoint = decodeString(memoryStream);
            this.P2EndPoint = decodeString(memoryStream);
            this.p1Name = decodeString(memoryStream);
            this.p2Name = decodeString(memoryStream);
            this.p1Win = decodeShort(memoryStream);
            this.p2Win = decodeShort(memoryStream);
            this.p1Lose = decodeShort(memoryStream);
            this.p2Lose = decodeShort(memoryStream);
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }

        public short getGameId() { return this.gameId; }
        public short getP1Id() { return this.p1Id; }
        public short getP2Id() { return this.p2Id; }
        public string getP1EndPoint() { return this.P1EndPoint; }
        public string getP2EndPoint() { return this.P2EndPoint; }
        public string getP1Name() { return this.p1Name; }
        public string getP2Name() { return this.p2Name; }
        public bool getP1Win() {
            if (this.p1Win == 1) return true;
            return false;
        }
        public bool getP2Win() {
            if (this.p2Win == 1) return true;
            return false;
        }
        public bool getP1Lose() {
            if (this.p1Lose == 1) return true;
            return false;
        }
        public bool getP2Lose() {
            if (this.p2Lose == 1) return true;
            return false;
        }
    }
}
