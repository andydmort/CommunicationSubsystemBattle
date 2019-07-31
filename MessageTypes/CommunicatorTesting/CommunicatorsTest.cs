using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Communicators;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CommunicatorTesting
{
    [TestClass]
    public class CommunicatorsTest
    {

        static Byte[] recievedData = new byte[] { 0x2 };

        static UdpCommunicator udpCom1 = new UdpCommunicator("0.0.0.0", 2201);

        [TestMethod]
        public void UdpClientSendAndRecieve()
        {

            Byte[] data = new byte[] { 0x20, 0x21, 0x22 };

            Thread thread = new Thread(threadFunc);
            thread.Start();

            udpCom1.setPeer("127.0.0.1", 2201);
            udpCom1.send(data);

            System.Threading.Thread.Sleep(1000); //Wait 1 seconds.

            Assert.AreEqual(data[0], recievedData[0]);

        }

        static void threadFunc()
        {
            recievedData = udpCom1.recieve();
        }
    }
}
