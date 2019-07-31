using System;
using System.Linq;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageTests
{
    [TestClass]
    public class ErrorMessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ErrorMessageTest));
        [TestMethod]
        public void TestEncodeDecode()
        {
            int errorCode = 1063;
            Identifier messageNum = new Identifier(2,3);
            Identifier conversationId = new Identifier(3,2);

            var outGoing1 = new ErrorMessage(errorCode, messageNum, conversationId);
            var outGoing2 = new ErrorMessage();

            byte[] bytes1 = outGoing1.encode();
            byte[] bytes2 = outGoing2.encode(errorCode, messageNum, conversationId);

            var MT1 = Message.decodeMessageType(bytes1);
            var MT2 = Message.decodeMessageType(bytes2);

            Assert.AreEqual((short)12, MT1);
            Assert.AreEqual((short)12, MT2);

            var errorMessage1 = (ErrorMessage)Message.decoder(bytes1);
            var errorMessage2 = (ErrorMessage)Message.decoder(bytes2);

            Assert.AreEqual(errorCode, errorMessage1.getErrorNumber());
            Assert.AreEqual(errorCode, errorMessage2.getErrorNumber());

            var testIdentifier1 = errorMessage1.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier1.getSeq());
            var testIdentifier2 = errorMessage2.getMessageNumber();
            Assert.IsInstanceOfType(testIdentifier2, typeof(Identifier));
            Assert.AreEqual(messageNum.getPid(), testIdentifier2.getPid());
            Assert.AreEqual(messageNum.getSeq(), testIdentifier2.getSeq());

            testIdentifier1 = errorMessage1.getConversationId();
            Assert.IsInstanceOfType(testIdentifier1, typeof(Identifier));
            Assert.AreEqual(conversationId.getPid(), testIdentifier1.getPid());
            Assert.AreEqual(conversationId.getSeq(), testIdentifier1.getSeq());
            testIdentifier2 = errorMessage2.getConversationId();
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
