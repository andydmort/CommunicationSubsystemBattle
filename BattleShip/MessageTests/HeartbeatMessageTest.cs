using System;
using System.Linq;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTests
{
    [TestClass]
    public class HeartbeatMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HeartbeatMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short id = 2;
            byte gORp = 1;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(54,5432);

            var outGoing1 = new HeartbeatMessage(id, gORp, messageNum, conversationId);
            var outGoing2 = new HeartbeatMessage();
            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(id, gORp, messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);
            Assert.AreEqual((short)4, MT1);
            Assert.AreEqual((short)4, MT2);

            var heartbeat1 = (HeartbeatMessage)Message.decoder(bytes1);
            var heartbeat2 = (HeartbeatMessage)Message.decoder(bytes2);
            Assert.AreEqual((short)2, heartbeat1.getID());
            Assert.AreEqual((byte)1, heartbeat1.getGORp());
            Assert.AreEqual((short)2, heartbeat2.getID());
            Assert.AreEqual((byte)1, heartbeat2.getGORp());

            var testIdentifier1 = heartbeat1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = heartbeat2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = heartbeat1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = heartbeat2.getConversationId();
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
