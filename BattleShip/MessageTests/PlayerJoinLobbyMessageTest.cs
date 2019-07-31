using System;
using System.Linq;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTests
{
    [TestClass]
    public class PlayerJoinLobbyMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PlayerJoinLobbyMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            string playerName = "Joe Ingle";
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(4,1);

            var outGoing1 = new PlayerJoinLobbyMessage(playerName,"Public Key", messageNum, conversationId);
            var outGoing2 = new PlayerJoinLobbyMessage();

            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(playerName,"Public Key", messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);

            Assert.AreEqual((short)2, MT1);
            Assert.AreEqual((short)2, MT2);

            var playerJoinLobby1 = (PlayerJoinLobbyMessage)Message.decoder(bytes1);
            var playerJoinLobby2 = (PlayerJoinLobbyMessage)Message.decoder(bytes2);

            Assert.AreEqual(playerName, playerJoinLobby1.getPlayerName());
            Assert.AreEqual(playerName, playerJoinLobby2.getPlayerName());

            var testIdentifier1 = playerJoinLobby1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = playerJoinLobby2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = playerJoinLobby1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = playerJoinLobby2.getConversationId();
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
