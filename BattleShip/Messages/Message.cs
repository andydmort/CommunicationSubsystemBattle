using System;
using System.Net;
using System.Text;
using System.IO;

namespace Messages
{
    public abstract class Message
    {
        public short messageType { get; set; }
        public Identifier MessageNumber { get; set; }
        public Identifier ConversationId { get; set; }

        public abstract byte[] encode();
        public abstract void decode(byte[] bytes);

        public void encodeIdentifier(Stream memoryStream, Identifier identifier)
        {
            var bytesPID = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(identifier.getPid()));
            memoryStream.Write(bytesPID, 0, bytesPID.Length);
            var bytesSEQ = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(identifier.getSeq()));
            memoryStream.Write(bytesSEQ, 0, bytesSEQ.Length);
        }
        public void encodeShort(Stream memoryStream, short value) {
            var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        public void encodeInt(Stream memoryStream, int value) {
            var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }
        public void encodeString(Stream memoryStream, string value) {
            var bytes_string = Encoding.BigEndianUnicode.GetBytes(value);
            this.encodeShort(memoryStream, (short) bytes_string.Length);
            memoryStream.Write(bytes_string, 0, bytes_string.Length);
        }

        public static short decodeShort(MemoryStream memoryStream){
            var bytes = new byte[2];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
        }

        public int decodeInt(Stream memoryStream){
            var bytes = new byte[4];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length) {
                throw new Exception("Tried to decode an int and failed");
            }
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }
        public string decodeString(MemoryStream memoryStream){
            var result = string.Empty;
            var length = decodeShort(memoryStream);
            if (length <= 0) return result;

            var bytes = new byte[length];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != length) {
                throw new Exception("Tried to decode a string and failed");
            }
            result = Encoding.BigEndianUnicode.GetString(bytes, 0, bytes.Length);
            return result;
        }
        public Identifier decodeIdentifier(MemoryStream memoryStream)
        {
            short PID = decodeShort(memoryStream);
            short SEQ = decodeShort(memoryStream);
            var identifier = new Identifier(PID, SEQ);
            return identifier;
        }
        public Identifier getMessageNumber() { return this.MessageNumber; }
        public Identifier getConversationId() { return this.ConversationId; }

        public static short decodeMessageType(byte[] byteArray)
        {
            var memoryStream = new MemoryStream(byteArray);
            var bytes = new byte[2];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length) {
                throw new Exception("Tried to decode a messageType and failed");
            }
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
        }

        public static Message decoder(byte[] bytes)
        {
            short MT = Message.decodeMessageType(bytes);
            Array.Copy(bytes, 2, bytes, 0, bytes.Length - 2);
            switch(MT)
            {
                case 1:
                    var gmJoinLobby = new GMJoinLobbyMessage();
                    gmJoinLobby.decode(bytes);
                    return gmJoinLobby;
                case 2:
                    var playerJoinLobby = new PlayerJoinLobbyMessage();
                    playerJoinLobby.decode(bytes);
                    return playerJoinLobby;
                case 3:
                    var assigIdMessage = new AssignIdMessage();
                    assigIdMessage.decode(bytes);
                    return assigIdMessage;
                case 4:
                    var heartbeat = new HeartbeatMessage();
                    heartbeat.decode(bytes);
                    return heartbeat;
                case 5:
                    var ack = new AckMessage();
                    ack.decode(bytes);
                    return ack;
                case 6:
                    var playerId = new PlayerIdMessage();
                    playerId.decode(bytes);
                    return playerId;
                case 7:
                    var gameId = new GameIdMessage();
                    gameId.decode(bytes);
                    return gameId;
                case 8:
                    var board = new BoardMessage();
                    board.decode(bytes);
                    return board;
                case 9:
                    var shot = new ShotMessage();
                    shot.decode(bytes);
                    return shot;
                case 10:
                    var result = new ResultMessage();
                    result.decode(bytes);
                    return result;
                case 11:
                    var endGameResult = new EndGameResultMessage();
                    endGameResult.decode(bytes);
                    return endGameResult;
                case 12:
                    var errorMessage = new ErrorMessage();
                    errorMessage.decode(bytes);
                    return errorMessage;
                default:
                    throw new Exception("Could not read messageType " + MT + " in switch statement");
            }
        }
    }
}
