using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System.Linq;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class ShotMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ShotMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short Xcord = 2;
            short Ycord = 2;
            short gameId = 12;
            short playerId = 1;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(44,449);

            var outGoing1 = new ShotMessage(Xcord, Ycord, gameId, playerId, messageNum, conversationId);
            var outGoing2 = new ShotMessage();

            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(Xcord, Ycord, gameId, playerId, messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);

            Assert.AreEqual((short)9, MT1);
            Assert.AreEqual((short)9, MT2);

            var shot1 = (ShotMessage)Message.decoder(bytes1);
            var shot2 = (ShotMessage)Message.decoder(bytes2);
            Assert.AreEqual(Xcord, shot1.getXcord());
            Assert.AreEqual(Xcord, shot2.getXcord());
            Assert.AreEqual(Ycord, shot1.getYcord());
            Assert.AreEqual(Ycord, shot2.getYcord());
            Assert.AreEqual(gameId, shot1.getGameId());
            Assert.AreEqual(gameId, shot2.getGameId());
            Assert.AreEqual(playerId, shot1.getPlayerId());
            Assert.AreEqual(playerId, shot2.getPlayerId());

            var testIdentifier1 = shot1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = shot2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = shot1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = shot2.getConversationId();
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
