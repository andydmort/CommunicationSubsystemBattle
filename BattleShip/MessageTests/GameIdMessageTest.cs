using System;
using System.Linq;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTests
{
    [TestClass]
    public class GameIdMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameIdMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short gameId = 1;
            short p1id = 2;
            short p2id = 3;
            string GMendpoint = "127.0.0.1:12343";
            string p1name = "Jerry";
            string p2name = "John";
            bool p1win = true;
            bool p2win = false;
            bool p1lose = true;
            bool p2lose = false;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(231,232);

            var OutGoing1 = new GameIdMessage(gameId, p1id, p2id, GMendpoint, p1name, p2name, p1win, p2win, p1lose, p2lose, messageNum, conversationId);
            var OutGoing2 = new GameIdMessage();

            byte[] bytes1 = OutGoing1.encode();
            byte[] bytes2 = OutGoing2.encode(gameId, p1id, p2id, GMendpoint, p1name, p2name, p1win, p2win, p1lose, p2lose, messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);

            Assert.AreEqual((short)7, MT1);
            Assert.AreEqual((short)7, MT2);

            var gameIdMessage1 = (GameIdMessage)Message.decoder(bytes1);
            var gameIdMessage2 = (GameIdMessage)Message.decoder(bytes2);

            Assert.AreEqual(gameId, gameIdMessage1.getGameId());
            Assert.AreEqual(gameId, gameIdMessage2.getGameId());
            Assert.AreEqual(p1id, gameIdMessage1.getP1Id());
            Assert.AreEqual(p1id, gameIdMessage2.getP1Id());
            Assert.AreEqual(p2id, gameIdMessage1.getP2Id());
            Assert.AreEqual(p2id, gameIdMessage2.getP2Id());
            Assert.AreEqual(GMendpoint, gameIdMessage1.getGMEndpoint());
            Assert.AreEqual(GMendpoint, gameIdMessage2.getGMEndpoint());
            Assert.AreEqual(p1name, gameIdMessage1.getP1Name());
            Assert.AreEqual(p1name, gameIdMessage2.getP1Name());
            Assert.AreEqual(p2name, gameIdMessage1.getP2Name());
            Assert.AreEqual(p2name, gameIdMessage2.getP2Name());
            Assert.AreEqual(p1win, gameIdMessage1.getP1Win());
            Assert.AreEqual(p1win, gameIdMessage2.getP1Win());
            Assert.AreEqual(p2win, gameIdMessage1.getP2Win());
            Assert.AreEqual(p2win, gameIdMessage2.getP2Win());
            Assert.AreEqual(p1lose, gameIdMessage1.getP1Lose());
            Assert.AreEqual(p2lose, gameIdMessage2.getP2Lose());
            Assert.AreEqual(p2lose, gameIdMessage1.getP2Lose());
            Assert.AreEqual(p1lose, gameIdMessage2.getP1Lose());

            var testIdentifier1 = gameIdMessage1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = gameIdMessage2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = gameIdMessage1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = gameIdMessage2.getConversationId();
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
