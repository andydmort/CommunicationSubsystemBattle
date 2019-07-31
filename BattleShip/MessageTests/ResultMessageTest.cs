using System;
using System.Linq;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class ResultMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ResultMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            bool hit = true;
            short gameId = 123;
            bool win = true;
            bool end = false;
            bool myTurn = true;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(123,1235);

            var outGoing1 = new ResultMessage(hit, gameId, win, end, myTurn,-1,-1, messageNum, conversationId);
            var outGoing2 = new ResultMessage();

            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(hit, gameId, win, end, myTurn,-1,-1,  messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1); 
            var MT2 = Message.decodeMessageType(bytes2); 
            Assert.AreEqual((short)10, MT1);
            Assert.AreEqual((short)10, MT2);

            var result1 = (ResultMessage)Message.decoder(bytes1);
            var result2 = (ResultMessage)Message.decoder(bytes2);

            Assert.AreEqual(hit, result1.getHit());
            Assert.AreEqual(hit, result2.getHit());
            Assert.AreEqual(gameId, result1.getGameId());
            Assert.AreEqual(gameId, result2.getGameId());
            Assert.AreEqual(win, result1.getWin());
            Assert.AreEqual(win, result2.getWin());
            Assert.AreEqual(end, result1.getEnd());
            Assert.AreEqual(end, result2.getEnd());
            Assert.AreEqual(myTurn, result1.getMyTurn());
            Assert.AreEqual(myTurn, result2.getMyTurn());

            var testIdentifier1 = result1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = result2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = result1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = result2.getConversationId();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier2.getSeq());

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
