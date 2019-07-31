using System;
using System.Net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using System.Text;

namespace MessageTests
{
    [TestClass]
    public class MessageTest
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MessageTest));
        [TestMethod]
        public void testDecoder()
        {
            short test = 1;
            byte[] bytes0 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(test));
            byte[] bytes1 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(test));
            byte[] bytes2 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(test));
            byte[] bytes3 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(test));
            int testInt = 2;
            byte[] bytes4 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(testInt));
            string testString = "public key";
            byte[] bytes6 = Encoding.BigEndianUnicode.GetBytes(testString);
            byte[] bytes5 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) bytes6.Length));


            byte[] bytes = new byte[bytes0.Length + bytes1.Length + bytes2.Length + bytes3.Length + bytes4.Length + bytes5.Length + bytes6.Length];

            Array.Copy(bytes0, 0, bytes, 0, bytes0.Length);
            Array.Copy(bytes1, 0, bytes, 2, bytes1.Length);
            Array.Copy(bytes2, 0, bytes, 4, bytes2.Length);
            Array.Copy(bytes3, 0, bytes, 6, bytes3.Length);
            Array.Copy(bytes4, 0, bytes, 8, bytes4.Length);
            Array.Copy(bytes5, 0, bytes, 12, bytes5.Length);
            Array.Copy(bytes5, 0, bytes, 14, bytes5.Length);

            var message = Message.decoder(bytes);
            Assert.IsInstanceOfType(message, typeof(Messages.GMJoinLobbyMessage));

            try
            {
                test    = 99; // this message does not exist
                bytes   = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(test));
                message = Message.decoder(bytes);
                Assert.Fail(); // getting here means an exception was not thrown

            } catch (Exception e)
            {
                Logger.Debug($"Test passed: {e}");
            }
        }
    }
}
