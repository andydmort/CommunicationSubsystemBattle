using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System.Linq;
using System;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class PlayerIdMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PlayerIdMessageTest));
        [TestMethod]
        public void TestEndodeDecode()
        {
            short gameId = 1;
            short P1id = 2;
            short P2id = 3;
            string Endpoint1 = "123.456.789:12001";
            string Endpoint2 = "123.456.789:12002";
            string P1Name = "John";
            string P2Name = "Jingle Hiemer Schmidt";
            bool P1win = true;
            bool P2win = false;
            bool P1lose = false;
            bool P2lose = true;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(5,2);
            // test initialization 
            var outGoing1 = new PlayerIdMessage();
            var outGoing2 = new PlayerIdMessage(gameId, P1id, P2id, Endpoint1, Endpoint2, P1Name, P2Name, P1win, P2win, P1lose, P2lose, messageNum, conversationId);
            byte[] bytes1 = outGoing1.encode(gameId, P1id, P2id, Endpoint1, Endpoint2, P1Name, P2Name, P1win, P2win, P1lose, P2lose, messageNum, conversationId);
            byte[] bytes2 = outGoing2.encode();
            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);
            Assert.AreEqual((short)6, MT1);
            Assert.AreEqual((short)6, MT2);
            PlayerIdMessage playerIdMessage1 = (PlayerIdMessage) Message.decoder(bytes1);
            PlayerIdMessage playerIdMessage2 = (PlayerIdMessage) Message.decoder(bytes2);

            Assert.AreEqual(gameId, playerIdMessage1.getGameId());
            Assert.AreEqual(P1id, playerIdMessage1.getP1Id());
            Assert.AreEqual(P2id, playerIdMessage1.getP2Id());
            Assert.AreEqual(Endpoint1, playerIdMessage1.getP1EndPoint());
            Assert.AreEqual(Endpoint2, playerIdMessage1.getP2EndPoint());
            Assert.AreEqual(P1Name, playerIdMessage1.getP1Name());
            Assert.AreEqual(P2Name, playerIdMessage1.getP2Name());
            Assert.AreEqual(P1win, playerIdMessage1.getP1Win());
            Assert.AreEqual(P1lose, playerIdMessage1.getP1Lose());
            Assert.AreEqual(P2win, playerIdMessage1.getP2Win());
            Assert.AreEqual(P2lose, playerIdMessage1.getP2Lose());
            Assert.AreEqual(gameId, playerIdMessage2.getGameId());
            Assert.AreEqual(P1id, playerIdMessage2.getP1Id());
            Assert.AreEqual(P2id, playerIdMessage2.getP2Id());
            Assert.AreEqual(Endpoint1, playerIdMessage2.getP1EndPoint());
            Assert.AreEqual(Endpoint2, playerIdMessage2.getP2EndPoint());
            Assert.AreEqual(P1Name, playerIdMessage2.getP1Name());
            Assert.AreEqual(P2Name, playerIdMessage2.getP2Name());
            Assert.AreEqual(P1win, playerIdMessage2.getP1Win());
            Assert.AreEqual(P1lose, playerIdMessage2.getP1Lose());
            Assert.AreEqual(P2win, playerIdMessage2.getP2Win());
            Assert.AreEqual(P2lose, playerIdMessage2.getP2Lose());

            var testIdentifier1 = playerIdMessage1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());

            var testIdentifier2 = playerIdMessage1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = playerIdMessage2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());

            testIdentifier2 = playerIdMessage2.getConversationId();
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
