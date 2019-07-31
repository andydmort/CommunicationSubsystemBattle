using System;
using System.Net;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public abstract class Message
    {
        protected short messageType;

        public abstract byte[] encode();
        public abstract void decode(byte[] bytes);

        public void encodeShort(Stream memoryStream, short value) {
            var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            memoryStream.Write(bytes, 0, bytes.Length);
        }
        public void encodeBool(Stream memoryStream, bool value) {
            //var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            byte[] bytes;
            if (value) bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1));
            else bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0));
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

        public short decodeShort(Stream memoryStream){
            var bytes = new byte[2];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length) {
                throw new Exception("Tried to decode a short and failed");
            }
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
        public bool decodeBool(Stream memoryStream){
            var bytes = new byte[1];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length) {
                throw new Exception("Tried to decode a boolean and failed");
            }
            short val = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
            bool decodedBool = false;
            if (val == 1) decodedBool = true;
            return decodedBool;
        }
        public string decodeString(Stream memoryStream){
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

        public static short decodeMessageType(Stream memoryStream){
            var bytes = new byte[2];
            var bytesRead = memoryStream.Read(bytes, 0, bytes.Length);
            if (bytesRead != bytes.Length) {
                throw new Exception("Tried to decode a short and failed");
            }
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 0));
        }

        public static Message decoder(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            short MT = Message.decodeMessageType(memoryStream);
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
                case 9:
                    var shot = new ShotMessage();
                    shot.decode(bytes);
                    return shot;
                case 10:
                    var result = new ResultMessage();
                    result.decode(bytes);
                    return result;
                default:
                    throw new Exception("Could not read messageType " + MT + " in message decoder");
            }
        }
    }
}
