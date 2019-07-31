using System;
using System.Linq;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTests
{
    [TestClass]
    public class EndGameResultMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EndGameResultMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short playerId = 23;
            short gameId = 34;
            Identifier messageNum = new Identifier(4, 2);
            Identifier conversationId = new Identifier(1, 1);

            var outGoing1 = new EndGameResultMessage(playerId, gameId, messageNum, conversationId);
            var outGoing2 = new EndGameResultMessage();

            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(playerId, gameId, messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);

            Assert.AreEqual((short)11, MT1);
            Assert.AreEqual((short)11, MT2);

            var endGameResult1 = (EndGameResultMessage)Message.decoder(bytes1);
            var endGameResult2 = (EndGameResultMessage)Message.decoder(bytes2);

            Assert.AreEqual(playerId, endGameResult1.getplayerId());
            Assert.AreEqual(playerId, endGameResult2.getplayerId());
            Assert.AreEqual(gameId, endGameResult1.getGameId());
            Assert.AreEqual(gameId, endGameResult2.getGameId());

            var testIdentifier1 = endGameResult1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = endGameResult2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = endGameResult1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = endGameResult2.getConversationId();
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
