using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System.Linq;
using System;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class AckMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AckMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short id = 2;
            byte gORp = 1;
            Identifier messageNum = new Identifier(2,3);
            Identifier ConvoId = new Identifier(0,1);

            var outGoing1 = new AckMessage(id, gORp, messageNum, ConvoId);
            var outGoing2 = new AckMessage();
            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(id, gORp, messageNum, ConvoId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);
            Assert.AreEqual((short)5, MT1);
            Assert.AreEqual((short)5, MT2);

            var ackMessage1 = (AckMessage)Message.decoder(bytes1);
            var ackMessage2 = (AckMessage)Message.decoder(bytes2);
            Assert.AreEqual((short)2, ackMessage1.getId());
            Assert.AreEqual((byte)1, ackMessage1.getGORp());
            Assert.AreEqual((short)2, ackMessage2.getId());
            Assert.AreEqual((byte)1, ackMessage2.getGORp());

            var testIdentifier1 = ackMessage1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = ackMessage2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = ackMessage1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(ConvoId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(ConvoId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = ackMessage2.getConversationId();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(ConvoId.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(ConvoId.getSeq(), testIdentifier2.getSeq());

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
