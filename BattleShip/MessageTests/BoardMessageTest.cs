using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Messages;
using System;
using System.Linq;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class BoardMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BoardMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short gameId = 1;
            List<List<short>> grid = new List<List<short>>();
            Identifier messageNum = new Identifier(1, 2);
            Identifier convoId = new Identifier(12, 22);

            for (int i = 0; i < 8; i++)
            {
                List<short> row = new List<short>();
                for (int j = 0; j < 8; j++)
                {
                    if (j % 2 == 0) row.Add(1);
                    else row.Add(0);
                }
                grid.Add(row);
            }

            var outGoing = new BoardMessage(gameId, grid, messageNum, convoId);
            byte[] bytes1 = outGoing.encode();

            var MT = Message.decodeMessageType(bytes1);
            Assert.AreEqual((short)8, MT);

            var board = (BoardMessage)Message.decoder(bytes1);
            Assert.AreEqual(gameId, board.getGameId());
            Assert.IsInstanceOfType(board.getGrid(), typeof(List<List<short>>)); 
            
            // test columns
            Assert.AreEqual(grid[0][0], board.getGrid()[0][0]);
            Assert.AreEqual(grid[1][0], board.getGrid()[1][0]);
            Assert.AreEqual(grid[2][0], board.getGrid()[2][0]);
            Assert.AreEqual(grid[3][0], board.getGrid()[3][0]);
            Assert.AreEqual(grid[4][0], board.getGrid()[4][0]);
            Assert.AreEqual(grid[5][0], board.getGrid()[5][0]);
            Assert.AreEqual(grid[6][0], board.getGrid()[6][0]);
            Assert.AreEqual(grid[7][0], board.getGrid()[7][0]);
            Assert.AreEqual(grid[0][1], board.getGrid()[0][1]);
            Assert.AreEqual(grid[1][1], board.getGrid()[1][1]);
            Assert.AreEqual(grid[2][1], board.getGrid()[2][1]);
            Assert.AreEqual(grid[3][1], board.getGrid()[3][1]);
            Assert.AreEqual(grid[4][1], board.getGrid()[4][1]);
            Assert.AreEqual(grid[5][1], board.getGrid()[5][1]);
            Assert.AreEqual(grid[6][1], board.getGrid()[6][1]);
            Assert.AreEqual(grid[7][1], board.getGrid()[7][1]);
            Assert.AreEqual(grid[0][2], board.getGrid()[0][2]);
            Assert.AreEqual(grid[1][2], board.getGrid()[1][2]);
            Assert.AreEqual(grid[2][2], board.getGrid()[2][2]);
            Assert.AreEqual(grid[3][2], board.getGrid()[3][2]);
            Assert.AreEqual(grid[4][2], board.getGrid()[4][2]);
            Assert.AreEqual(grid[5][2], board.getGrid()[5][2]);
            Assert.AreEqual(grid[6][2], board.getGrid()[6][2]);
            Assert.AreEqual(grid[7][2], board.getGrid()[7][2]);
            Assert.AreEqual(grid[0][7], board.getGrid()[0][7]);
            Assert.AreEqual(grid[1][7], board.getGrid()[1][7]);
            Assert.AreEqual(grid[2][7], board.getGrid()[2][7]);
            Assert.AreEqual(grid[3][7], board.getGrid()[3][7]);
            Assert.AreEqual(grid[4][7], board.getGrid()[4][7]);
            Assert.AreEqual(grid[5][7], board.getGrid()[5][7]);
            Assert.AreEqual(grid[6][7], board.getGrid()[6][7]);
            Assert.AreEqual(grid[7][7], board.getGrid()[7][7]);

            // test rows;
            Assert.AreEqual(grid[3][3], board.getGrid()[3][3]);
            Assert.AreEqual(grid[3][4], board.getGrid()[3][4]);
            Assert.AreEqual(grid[3][5], board.getGrid()[3][5]);
            Assert.AreEqual(grid[3][6], board.getGrid()[3][6]);
            Assert.AreEqual(grid[4][3], board.getGrid()[4][3]);
            Assert.AreEqual(grid[4][4], board.getGrid()[4][4]);
            Assert.AreEqual(grid[4][5], board.getGrid()[4][5]);
            Assert.AreEqual(grid[4][6], board.getGrid()[4][6]);
            Assert.AreEqual(grid[5][3], board.getGrid()[5][3]);
            Assert.AreEqual(grid[5][4], board.getGrid()[5][4]);
            Assert.AreEqual(grid[5][5], board.getGrid()[5][5]);
            Assert.AreEqual(grid[5][6], board.getGrid()[5][6]);
            Assert.AreEqual(grid[6][3], board.getGrid()[6][3]);
            Assert.AreEqual(grid[6][4], board.getGrid()[6][4]);
            Assert.AreEqual(grid[6][5], board.getGrid()[6][5]);
            Assert.AreEqual(grid[6][6], board.getGrid()[6][6]);
            Assert.AreEqual(grid[7][3], board.getGrid()[7][3]);
            Assert.AreEqual(grid[7][4], board.getGrid()[7][4]);
            Assert.AreEqual(grid[7][5], board.getGrid()[7][5]);
            Assert.AreEqual(grid[7][6], board.getGrid()[7][6]);

            var testIdentifier1 = board.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());

            var testIdentifier2 = board.getConversationId();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(convoId.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(convoId.getSeq(), testIdentifier2.getSeq());

            byte[] badBytes = new byte[2];
            badBytes[0] = 1;
            badBytes[1] = 1;
            badBytes = bytes1.Concat(badBytes).ToArray();
            try
            {
                Message.decoder(badBytes);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Logger.Debug($"Test passed: {e}");
            }
        }
    }
}
