using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Net;
using GameManager;
using Lobby;
using CommSubSys;
using Messages;
using Player;
using System.Collections.Generic;

namespace ProtocolTests
{
    [TestClass]
    public class GameReadyTest
    {
        public class dummyConversationFactory : ConversationFactory
        {
            public dummyConversationFactory()
            {

            }
            public override void Initialize()
            {
                //Doing nothing here
            }
        }

        [TestMethod]
        public void BoardConvoRetries()
        {
            dummyConversationFactory dumConvoFact = new dummyConversationFactory();
            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState playerAppState = null; 
            try
            {
                playerAppState= new PlayerAppState(waitingRoom);
            }
            catch(Exception e)
            {

            }
            SubSystem commSubSys = new SubSystem(dumConvoFact, playerAppState);

            Board boardConvo = new Board();
            boardConvo.SubSystem = commSubSys;
            boardConvo.RemoteEndPoint =  new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            boardConvo.Launch();

            Thread.Sleep(1000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 1);

            Thread.Sleep(2000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 2);

            Thread.Sleep(2000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 3);

            //Make sure those messages are the same. 
            Envelope env1;
            if(!commSubSys.outQueue.TryDequeue(out env1))
            {
                Assert.Fail();
            }
            Envelope env2;
            if(!commSubSys.outQueue.TryDequeue(out env2))
            {
                Assert.Fail();
            }
            Envelope env3;
            if(!commSubSys.outQueue.TryDequeue(out env3))
            {
                Assert.Fail();
            }

            //Make sure the message in the outQ are the same. 
            Assert.IsTrue(env1.Message.ConversationId.Equals(env2.Message.ConversationId) && env1.Message.MessageNumber.Equals(env2.Message.MessageNumber));
            Assert.IsTrue(env1.Message.ConversationId.Equals(env3.Message.ConversationId) && env1.Message.MessageNumber.Equals(env3.Message.MessageNumber));

            //Close the communicators for next test. 
            commSubSys.udpcomm.closeCommunicator();
            commSubSys.tcpcomm.closeCommunicator();
        }

        [TestMethod]
        public void RespondBoardRespondingToRetries()
        {
            //Create a SubSystem with dummy factory. 
            dummyConversationFactory dumConvoFact = new dummyConversationFactory();
            GMAppState gmAppState = new GMAppState();
            SubSystem commSubSys = new SubSystem(dumConvoFact, gmAppState);
            //Note: we dont want to start the threads of the subSystem. 

            //Create Conversation
            RespondBoard respondBoard = new RespondBoard();
            respondBoard.SubSystem = commSubSys;
            respondBoard.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            //Create fake incoming env. 
            List<List<short>> grid = new List<List<short>>();
            Envelope IncomingEnv = new Envelope(new BoardMessage(1, grid, new Identifier(0, 1), new Identifier(0, 1)), new IPEndPoint(IPAddress.Parse("1.1.1.1"),1111), new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222), false);

            //Set the incoming env.
            respondBoard.IncomingEnvelope = IncomingEnv;
            respondBoard.recievedMessagesDict.TryAdd(IncomingEnv.Message.MessageNumber.ToString(), IncomingEnv);

            //Start the conversation
            respondBoard.Launch();

            Thread.Sleep(500);

            //check the outgoing message.
            Assert.IsTrue(commSubSys.outQueue.Count >= 1);

            //Give a duplicate message.
            respondBoard.Process(IncomingEnv);

            Thread.Sleep(500);

            //check the outgoing message.
            Assert.IsTrue(commSubSys.outQueue.Count >= 2);

            //get count of outgoing messages for later. 
            int countOfOutGoingMessages = commSubSys.outQueue.Count;

            //Wait for timeout time
            Thread.Sleep(2000);

            //Check that there are no new messages. 
            Assert.IsTrue(countOfOutGoingMessages == commSubSys.outQueue.Count);

            //Cheack that thread has ended.
            respondBoard.Process(IncomingEnv); //If thread was running outgoing messages should go up. 
            Thread.Sleep(500);
            Assert.IsTrue(countOfOutGoingMessages == commSubSys.outQueue.Count);

            //Close the communicators for next test. 
            commSubSys.udpcomm.closeCommunicator();
            commSubSys.tcpcomm.closeCommunicator();
        }

        [TestMethod]
        public void GameReadyFullProtocolTest()
        {
            //Create a SubSystem with dummy factory. 
            dummyConversationFactory dumConvoFact1 = new dummyConversationFactory();
            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState playerAppState = new PlayerAppState(waitingRoom);
            SubSystem commSubSys1 = new SubSystem(dumConvoFact1, playerAppState);
            //dummyCoversationFactory dumConvoFact2 = new dummyCoversationFactory();
            //SubSystem commSubSys2;
            //Note: Its a little strange that we are only using one subsystem here. Using two wont work because we have hard coded the Sockets that UDP and TCP are using and we can't bind to the same sockets.
            //Note: we dont want to start the threads of the subSystem. 

            //get the initial process id. This test will check that the conversation changes it. 
            int startingProcessId = commSubSys1.ProcessID;

            //Create Initiator conversation
            Board BoardConvo = new Board();
            BoardConvo.SubSystem = commSubSys1;
            BoardConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            ////Create Responder Conversation
            RespondBoard respondBoardConvo = new RespondBoard();
            GMAppState gmAppState = new GMAppState();
            SubSystem commSubSys2 = new SubSystem(dumConvoFact1, gmAppState);
            respondBoardConvo.SubSystem = commSubSys2;
            respondBoardConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222);

            //Start Initiator Conversation
            BoardConvo.Launch();

            //Wait for conversation to send message.
            Thread.Sleep(1000);

            //Check for outgoing message.
            Assert.IsTrue(commSubSys1.outQueue.Count >= 1);

            //Get message and put into responder conversation
            Envelope initiatorEnv;
            if (!commSubSys1.outQueue.TryDequeue(out initiatorEnv)) Assert.Fail();
            respondBoardConvo.IncomingEnvelope = initiatorEnv;
            respondBoardConvo.ConversationId = initiatorEnv.Message.ConversationId;

            //Start the responder conversation
            respondBoardConvo.Launch();

            //Wait for conversation to send message
            Thread.Sleep(500);

            //Check for outgoing message from responder conversation.
            Assert.IsTrue(commSubSys2.outQueue.Count >= 1);

            //Get responder reply and give to initiator. 
            Envelope responderEnv;
            if (!commSubSys2.outQueue.TryDequeue(out responderEnv)) Assert.Fail();
            BoardConvo.Process(responderEnv);

            //Check that the process Id was changed. 
            Assert.IsTrue(startingProcessId == commSubSys1.ProcessID);

            //Check that gmAppState and playerAppState are correct
            Assert.AreEqual(gmAppState.gameId, playerAppState.gameId);
            Assert.IsNotNull(gmAppState.P1grid);
            Assert.IsNotNull(playerAppState.planningGrid);
            Assert.AreEqual(gmAppState.P1grid, playerAppState.planningGrid);

            //Close the communicators for next test. 
            commSubSys1.udpcomm.closeCommunicator();
            commSubSys1.tcpcomm.closeCommunicator();
           
        }
    }
}
