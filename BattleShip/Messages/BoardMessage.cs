using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Messages
{
    public class BoardMessage : Message
    {
        private short gameId;
        private List<List<short>> grid;
        private short dimension; // need dimension for decoding

        public BoardMessage() { this.messageType = 8; }
        public BoardMessage(short gameId, List<List<short>> grid, Identifier messageNum, Identifier convoId)
        {
            this.messageType = 8;
            this.MessageNumber = messageNum;
            this.ConversationId = convoId;
            this.gameId = gameId;
            this.grid = grid;
            this.dimension = (short)grid.Count;
        }
        public override byte[] encode()
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, this.gameId);
            this.dimension = (short)grid.Count;
            encodeShort(memoryStream, this.dimension);
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    encodeShort(memoryStream, this.grid[i][j]);
                }
            }
            encodeIdentifier(memoryStream, this.MessageNumber);
            encodeIdentifier(memoryStream, this.ConversationId);
            return memoryStream.ToArray();
        }
        public byte[] encode(short gameId, List<List<short>> grid, Identifier identifier)
        {
            var memoryStream = new MemoryStream();
            encodeShort(memoryStream, this.messageType);
            encodeShort(memoryStream, gameId);
            var dimension = (short)grid.Count;
            encodeShort(memoryStream, dimension);
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    encodeShort(memoryStream, grid[i][j]);
                }
            }
            encodeIdentifier(memoryStream, identifier);
            return memoryStream.ToArray();
        }
        // need to have a dimension set before decoding
        public void setDimension(short d)
        {
            this.dimension = d;
        }
        public override void decode(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            this.gameId = decodeShort(memoryStream);
            this.dimension = decodeShort(memoryStream);
            this.grid = new List<List<short>>();
            for (int i = 0; i < this.dimension; i++)
            {
                List<short> row = new List<short>();
                for (int j = 0; j < this.dimension; j++)
                {
                    row.Add(decodeShort(memoryStream));
                }
                this.grid.Add(row);
            }
            this.MessageNumber = decodeIdentifier(memoryStream);
            this.ConversationId = decodeIdentifier(memoryStream);
        }
        public short getGameId() { return this.gameId; }
        public List<List<short>> getGrid() { return this.grid; }
    }
}
