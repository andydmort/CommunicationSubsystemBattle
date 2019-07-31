using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Messages;
using CommSubSys;
using System.Threading;

namespace ComSubSystemTests
{
    [TestClass]
    public class CommunicatorTests
    {

        [TestMethod]
        public void UdpCommTest()
        {
            //Create 4 queue for both systems
            EnvelopeQueue inQ1 = new EnvelopeQueue();
            EnvelopeQueue outQ1 = new EnvelopeQueue();
            EnvelopeQueue inQ2 = new EnvelopeQueue();
            EnvelopeQueue outQ2 = new EnvelopeQueue();

            //Create the communicators
            UdpCommunicator udpCom1 = new UdpCommunicator(inQ1, outQ1, new IPEndPoint(IPAddress.Loopback, 0));
            UdpCommunicator udpCom2 = new UdpCommunicator(inQ2, outQ2, new IPEndPoint(IPAddress.Loopback, 0));

            //Start communicators
            udpCom1.startThreads();
            udpCom2.startThreads();

            //Create a message to send. 
            AssignIdMessage mess = new AssignIdMessage(4,new byte[] { 0x1, 0x3 },new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));

            //Try to send a message between them
            Envelope env = new Envelope(mess, udpCom1.GetEndPoint(), udpCom2.GetEndPoint(), false);
            outQ1.Enqueue(env);

            //Wait for the message. 
            Thread.Sleep(4000);

            //Make sure we received something. 
            Assert.IsTrue(!inQ2.IsEmpty);

            //Check the message. 
            Envelope tmpEnv;
            inQ2.TryDequeue(out tmpEnv);

            Assert.AreEqual(mess.getId(), ((AssignIdMessage)tmpEnv.Message).getId());


            udpCom1.closeCommunicator();
            udpCom2.closeCommunicator();
        }

        [TestMethod]
        public void TcpComTest()
        {
            //Create the communicators
            //Create 4 queue for both systems
            EnvelopeQueue inQ1 = new EnvelopeQueue();
            EnvelopeQueue outQ1 = new EnvelopeQueue();

            EnvelopeQueue inQ2 = new EnvelopeQueue();
            EnvelopeQueue outQ2 = new EnvelopeQueue();

            TcpCommunicator tcpCom1 = new TcpCommunicator( inQ1,  outQ1, new IPEndPoint(IPAddress.Loopback, 2205));
            TcpCommunicator tcpCom2 = new TcpCommunicator( inQ2,  outQ2, new IPEndPoint(IPAddress.Loopback, 2206));

            //start communicator threads.
            tcpCom1.startThreads();
            tcpCom2.startThreads();

            //Create a message to send. 
            AssignIdMessage mess = new AssignIdMessage(5, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1,1));

            //Try to send a message between them
            Envelope env = new Envelope(mess, tcpCom1.getListenerEndPoint(), tcpCom2.getListenerEndPoint(), true);
            outQ1.Enqueue(env);

            //Wait for the message. 
            Thread.Sleep(4000);

            //Make sure we received something. 
            Assert.IsTrue(!inQ2.IsEmpty);


            //Check the message. 
            Envelope tmpEnv;
            inQ2.TryDequeue(out tmpEnv);

            Assert.AreEqual(mess.getId(), ((AssignIdMessage)tmpEnv.Message).getId());

            tcpCom1.closeCommunicator();
            tcpCom2.closeCommunicator();
        }

        [TestMethod]
        public void MultipleTcpComTest()
        {
             EnvelopeQueue inQA = new EnvelopeQueue();
             EnvelopeQueue outQA = new EnvelopeQueue();
             EnvelopeQueue inQB = new EnvelopeQueue();
             EnvelopeQueue outQB = new EnvelopeQueue();
             EnvelopeQueue inQC = new EnvelopeQueue();
             EnvelopeQueue outQC = new EnvelopeQueue();

            //Create the communicators
            TcpCommunicator tcpComA = new TcpCommunicator(inQA, outQA, new IPEndPoint(IPAddress.Loopback, 0));
            TcpCommunicator tcpComB = new TcpCommunicator(inQB, outQB, new IPEndPoint(IPAddress.Loopback, 0));
            TcpCommunicator tcpComC = new TcpCommunicator(inQC, outQC, new IPEndPoint(IPAddress.Loopback, 0));

            //start communicator threads
            tcpComA.startThreads();
            tcpComB.startThreads();
            tcpComC.startThreads();

            //Create a messages to send. 
            AssignIdMessage messAToB = new AssignIdMessage(5, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));
            AssignIdMessage messAToC = new AssignIdMessage(5, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 2), new Identifier(1, 2));
            AssignIdMessage messAToBAgain = new AssignIdMessage(5, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 3), new Identifier(1, 3));

            //Create Envelopes
            Envelope env1 = new Envelope(messAToB, tcpComA.getListenerEndPoint(), tcpComB.getListenerEndPoint(), true);
            Envelope env2 = new Envelope(messAToC, tcpComA.getListenerEndPoint(), tcpComC.getListenerEndPoint(), true);
            Envelope env3 = new Envelope(messAToBAgain, tcpComA.getListenerEndPoint(), tcpComB.getListenerEndPoint(), true);

            //Send Envelopes
            outQA.Enqueue(env1);
            outQA.Enqueue(env2);
            outQA.Enqueue(env3);

            //wait for messages. 
            Thread.Sleep(5000);

            //Make sure message were recieved. 
            Assert.IsTrue(inQB.Count == 2);
            Assert.IsTrue(inQC.Count == 1);

            //Check the envelopes
            Envelope env;

            inQB.TryDequeue(out env);
            Assert.AreEqual(messAToB.ConversationId.Seq, env.Message.ConversationId.Seq);

            inQB.TryDequeue(out env);
            Assert.AreEqual(messAToBAgain.ConversationId.Seq, env.Message.ConversationId.Seq);

            inQC.TryDequeue(out env);
            Assert.AreEqual(messAToC.ConversationId.Seq, env.Message.ConversationId.Seq);

            Console.WriteLine("Here");
            tcpComA.closeCommunicator();
            tcpComB.closeCommunicator();
            tcpComC.closeCommunicator();
        }

        [TestMethod]
        public void UdpAndTcpTest()
        {
            //Create 4 queue for both systems
            EnvelopeQueue inQ1A = new EnvelopeQueue();
            EnvelopeQueue outQ1A = new EnvelopeQueue();
            EnvelopeQueue inQ2A = new EnvelopeQueue();
            EnvelopeQueue outQ2A = new EnvelopeQueue();

            //Create the communicators
            TcpCommunicator tcpCom1 = new TcpCommunicator(inQ1A, outQ1A, new IPEndPoint(IPAddress.Loopback, 2210)); //Sender 1
            TcpCommunicator tcpCom2 = new TcpCommunicator(inQ2A, outQ2A, new IPEndPoint(IPAddress.Loopback, 2211)); //Reciever2 

            //Create the communicators
            UdpCommunicator udpCom1 = new UdpCommunicator(inQ1A, outQ1A, new IPEndPoint(IPAddress.Loopback, 2212)); //Sender 1
            UdpCommunicator udpCom2 = new UdpCommunicator(inQ2A, outQ2A, new IPEndPoint(IPAddress.Loopback, 2213)); //Reciever2

            //Start communicator threads
            tcpCom1.startThreads();
            tcpCom2.startThreads();
            udpCom1.startThreads();
            udpCom2.startThreads();

            //Create the messages
            AssignIdMessage mess1 = new AssignIdMessage(1, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));
            AssignIdMessage mess2 = new AssignIdMessage(2, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));
            AssignIdMessage mess3 = new AssignIdMessage(3, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));
            AssignIdMessage mess4 = new AssignIdMessage(4, new byte[] { 0x1, 0x3 }, new byte[] { 0x1, 0x2 }, new Identifier(1, 1), new Identifier(1, 1));

            //Make the Envelopes
            Envelope env1 = new Envelope(mess1, udpCom1.GetEndPoint(), udpCom2.GetEndPoint(), false);
            Envelope env2 = new Envelope(mess2, udpCom1.GetEndPoint(), udpCom2.GetEndPoint(), false);
            Envelope env3 = new Envelope(mess3, tcpCom1.getListenerEndPoint(), tcpCom2.getListenerEndPoint(), true);
            Envelope env4 = new Envelope(mess4, tcpCom1.getListenerEndPoint(), tcpCom2.getListenerEndPoint(), true);


            //Put in sending Q. 
            outQ1A.Enqueue(env1);
            outQ1A.Enqueue(env3);
            outQ1A.Enqueue(env2);
            outQ1A.Enqueue(env4);

            Thread.Sleep(3000); //Wait for messages to send. 

            //Pull out messages
            Envelope tmpEnv1;
            Envelope tmpEnv2;
            Envelope tmpEnv3;
            Envelope tmpEnv4;

            //Is there something in the Q?
            Assert.IsTrue(!inQ2A.IsEmpty);

            inQ2A.TryDequeue(out tmpEnv1);
            inQ2A.TryDequeue(out tmpEnv2);
            inQ2A.TryDequeue(out tmpEnv3);
            inQ2A.TryDequeue(out tmpEnv4);

            Console.WriteLine(((AssignIdMessage)tmpEnv1.Message).getId());
            Assert.IsTrue(1 == ((AssignIdMessage)tmpEnv1.Message).getId() | 2 == ((AssignIdMessage)tmpEnv1.Message).getId() | 3 == ((AssignIdMessage)tmpEnv1.Message).getId() | 4 == ((AssignIdMessage)tmpEnv1.Message).getId());
            Assert.IsTrue(1 == ((AssignIdMessage)tmpEnv2.Message).getId() | 2 == ((AssignIdMessage)tmpEnv2.Message).getId() | 3 == ((AssignIdMessage)tmpEnv2.Message).getId() | 4 == ((AssignIdMessage)tmpEnv2.Message).getId());
            Assert.IsTrue(1 == ((AssignIdMessage)tmpEnv3.Message).getId() | 2 == ((AssignIdMessage)tmpEnv3.Message).getId() | 3 == ((AssignIdMessage)tmpEnv3.Message).getId() | 4 == ((AssignIdMessage)tmpEnv3.Message).getId());
            Assert.IsTrue(1 == ((AssignIdMessage)tmpEnv4.Message).getId() | 2 == ((AssignIdMessage)tmpEnv4.Message).getId() | 3 == ((AssignIdMessage)tmpEnv4.Message).getId() | 4 == ((AssignIdMessage)tmpEnv4.Message).getId());

            tcpCom1.closeCommunicator();
            tcpCom2.closeCommunicator();
            udpCom1.closeCommunicator();
            udpCom2.closeCommunicator();

        }

        

    }
}
