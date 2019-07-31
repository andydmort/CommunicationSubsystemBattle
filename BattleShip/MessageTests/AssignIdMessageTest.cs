using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System.Linq;
using log4net;

namespace MessageTests
{
    [TestClass]
    public class AssignIdMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AssignIdMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            short id = 1;
            Identifier messageNumber = new Identifier(2,3);
            Identifier conversationId = new Identifier(3,2);
            // test initialization 
            var outGoing = new AssignIdMessage(id,new byte[] { 0x1, 0x3 },new byte[] { 0x1, 0x2 }, messageNumber, conversationId);
            byte[] bytes1 = outGoing.encode();

            var MT = Message.decodeMessageType(bytes1);
            Assert.AreEqual((short)3, MT);

            var assignIdMessage = (AssignIdMessage)Message.decoder(bytes1);
            Assert.AreEqual((short)1, assignIdMessage.getId());

            outGoing = new AssignIdMessage();
            byte[] bytes2 = outGoing.encode(id, new byte[] { 0x1, 0x2 }, new byte[] { 0x1, 0x2 }, messageNumber, conversationId);

            MT = Message.decodeMessageType(bytes2);
            Assert.AreEqual((short)3, MT);

            assignIdMessage = (AssignIdMessage)Message.decoder(bytes2);
            Assert.AreEqual((short)1, assignIdMessage.getId());

            var testIdentifier1 = assignIdMessage.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNumber.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNumber.getSeq(), testIdentifier1.getSeq());

            var testIdentifier2 = assignIdMessage.getConversationId();
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
