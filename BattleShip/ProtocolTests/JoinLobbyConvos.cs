using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Net;
using GameManager;
using Lobby;
using CommSubSys;
using Messages;
using Player;

namespace ProtocolTests
{
    [TestClass]
    public class JoinLobbyConvos
    {
        public class dummyCoversationFactory : ConversationFactory
        {
            public dummyCoversationFactory()
            {

            }
            public override void Initialize()
            {
                //Do nothing here. 
            }
        }

       

        [TestMethod]
        public void GMJoinLobbyConvoRetries()
        {
            //Create a SubSystem with dummy factory. 
            dummyCoversationFactory dumConvoFact = new dummyCoversationFactory();
            GMAppState lobbyAppState = new GMAppState();
            SubSystem commSubSys = new SubSystem(dumConvoFact, lobbyAppState);
            //Note: we dont want to start the threads of the subSystem. 

            //Create conversation
            GMJoinLobby GMJoinLobbyConversation = new GMJoinLobby();
            GMJoinLobbyConversation.SubSystem = commSubSys;
            GMJoinLobbyConversation.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111); //This is just a dummpy endpoint. Because the TCP and UDP thread are running it may actually send. 

            //Start conversation
            GMJoinLobbyConversation.Launch();

            //Wait one second. 
            Thread.Sleep(1000);

            //Check outQ for outgoing messages.
            Assert.IsTrue(commSubSys.outQueue.Count >= 1);

            //Wait for retry
            Thread.Sleep(2000);

            //Chekc outQ for multiple outgoing messages. 
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
        public void RespondJoinLobbyRespondingToRetreis()
        {
            //Create a SubSystem with dummy factory. 
            dummyCoversationFactory dumConvoFact = new dummyCoversationFactory();
            LobbyAppState lobbyAppState = new LobbyAppState();
            SubSystem commSubSys = new SubSystem(dumConvoFact, lobbyAppState);
            //Note: we dont want to start the threads of the subSystem. 

            //Create Conversation
            RespondJoinLobby resJoinLobbyConvo = new RespondJoinLobby();
            resJoinLobbyConvo.SubSystem = commSubSys;
            resJoinLobbyConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            //Create fake incoming env. 
            string strXML = "<RSAKeyValue><Modulus>CmZ5Hasfdldkjrjghtyremeddfjdn4738ejdjHGTFEeeeeeeeeeefjdkeldkfjguehendjeufi8ejdmGFDSqaalskebejdu==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            Envelope IncomingEnv = new Envelope(new GMJoinLobbyMessage(strXML, new Identifier(0, 1), new Identifier(0, 1),1111), new IPEndPoint(IPAddress.Parse("1.1.1.1"),1111), new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222),false);

            //Set the incoming env.
            resJoinLobbyConvo.IncomingEnvelope = IncomingEnv;
            resJoinLobbyConvo.recievedMessagesDict.TryAdd(IncomingEnv.Message.MessageNumber.ToString(), IncomingEnv);

            //Start the conversation
            resJoinLobbyConvo.Launch();

            Thread.Sleep(500);

            //check the outgoing message.
            Assert.IsTrue(commSubSys.outQueue.Count >= 1);

            //Give a duplicate message.
            resJoinLobbyConvo.Process(IncomingEnv);

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
            resJoinLobbyConvo.Process(IncomingEnv); //If thread was running outgoing messages should go up. 
            Thread.Sleep(500);
            Assert.IsTrue(countOfOutGoingMessages == commSubSys.outQueue.Count);

            //Close the communicators for next test. 
            commSubSys.udpcomm.closeCommunicator();
            commSubSys.tcpcomm.closeCommunicator();

        }

        [TestMethod]
        public void GMJoinLobbyFullProtocolTest()
        {

            //Create a SubSystem with dummy factory. 
            dummyCoversationFactory dumConvoFact1 = new dummyCoversationFactory();
            LobbyAppState lobbyAppState = new LobbyAppState();
            GMAppState gMAppState = new GMAppState();
            SubSystem commSubSys1 = new SubSystem(dumConvoFact1, lobbyAppState);
            SubSystem commSubSysGM = new SubSystem(dumConvoFact1, gMAppState, new IPEndPoint(IPAddress.Any,3001),new IPEndPoint(IPAddress.Any,3002));
            //dummyCoversationFactory dumConvoFact2 = new dummyCoversationFactory();
            //SubSystem commSubSys2;
            //Note: Its a little strange that we are only using one subsystem here. Using two wont work because we have hard coded the Sockets that UDP and TCP are using and we can't bind to the same sockets.
            //Note: we dont want to start the threads of the subSystem. 

            //get the initial process id. This test will check that the conversation changes it. 
            int startingProcessId = commSubSysGM.ProcessID;

            //Create Initiator conversation
            GMJoinLobby GMJoinLobbyConversation = new GMJoinLobby();
            GMJoinLobbyConversation.SubSystem = commSubSysGM;
            GMJoinLobbyConversation.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            ////Create Responder Conversation
            RespondJoinLobby resJoinLobbyConvo = new RespondJoinLobby();
            resJoinLobbyConvo.SubSystem = commSubSys1;
            resJoinLobbyConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222);

            //Start Initiator Conversation
            GMJoinLobbyConversation.Launch();

            //Wait for conversation to send message.
            Thread.Sleep(500);

            //Check for outgoing message.
            Assert.IsTrue(commSubSysGM.outQueue.Count >= 1);

            //Get message and put into responder conversation
            Envelope initiatorEnv;
            if (!commSubSysGM.outQueue.TryDequeue(out initiatorEnv)) Assert.Fail();
            initiatorEnv.From = commSubSysGM.udpcomm.GetEndPoint(); //Added to resolve a message. 
            resJoinLobbyConvo.IncomingEnvelope = initiatorEnv;
            resJoinLobbyConvo.ConversationId = initiatorEnv.Message.ConversationId;

            //Start the responder conversation
            resJoinLobbyConvo.Launch();

            //Wait for conversation to send message
            Thread.Sleep(500);

            //Check for outgoing message from responder conversation.
            Assert.IsTrue(commSubSys1.outQueue.Count >= 1);

            //Get responder reply and give to initiator. 
            Envelope responderEnv;
            if (!commSubSys1.outQueue.TryDequeue(out responderEnv)) Assert.Fail();
            GMJoinLobbyConversation.Process(responderEnv);

            //Check that the process Id was changed. Note: This is the 
            Assert.IsTrue(startingProcessId == commSubSysGM.ProcessID);
            Console.WriteLine(startingProcessId);
            Console.WriteLine(commSubSysGM.ProcessID);

            //Check that the GM was added to the Lobby.
            Assert.IsTrue(lobbyAppState.GMs.Count >= 1);

            //Close the communicators for next test. 
            commSubSys1.udpcomm.closeCommunicator();
            commSubSys1.tcpcomm.closeCommunicator();
           
        }

        [TestMethod]
        public void PlayerJoinLobbyConvoRetries()
        {
            //Create a SubSystem with dummy factory. 
            dummyCoversationFactory dumConvoFact = new dummyCoversationFactory();
            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState playerAppState = new PlayerAppState(waitingRoom);
            SubSystem commSubSysPlayer = new SubSystem(dumConvoFact, playerAppState);
            //Note: we dont want to start the threads of the subSystem. 

            //Set the name of the player
            playerAppState.playerName = "Tester Testerson";

            //Create conversation
            PlayerJoinLobby playerJoinLobbyConvo = new PlayerJoinLobby();
            playerJoinLobbyConvo.SubSystem = commSubSysPlayer;
            playerJoinLobbyConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111); //This is just a dummpy endpoint. Because the TCP and UDP thread are running it may actually send. 

            //Start conversation
            playerJoinLobbyConvo.Launch();

            //Wait one second. 
            Thread.Sleep(1000);

            //Check outQ for outgoing messages.
            Assert.IsTrue(commSubSysPlayer.outQueue.Count >= 1);

            //Wait for retry
            Thread.Sleep(2000);

            //Chekc outQ for multiple outgoing messages. 
            Assert.IsTrue(commSubSysPlayer.outQueue.Count >= 2);

            Thread.Sleep(2000);

            Assert.IsTrue(commSubSysPlayer.outQueue.Count >= 3);

            //Make sure those messages are the same. 
            Envelope env1;
            if (!commSubSysPlayer.outQueue.TryDequeue(out env1))
            {
                Assert.Fail();
            }
            Envelope env2;
            if (!commSubSysPlayer.outQueue.TryDequeue(out env2))
            {
                Assert.Fail();
            }
            Envelope env3;
            if (!commSubSysPlayer.outQueue.TryDequeue(out env3))
            {
                Assert.Fail();
            }


            //Make sure the message in the outQ are the same. 
            Assert.IsTrue(env1.Message.ConversationId.Equals(env2.Message.ConversationId) && env1.Message.MessageNumber.Equals(env2.Message.MessageNumber));
            Assert.IsTrue(env1.Message.ConversationId.Equals(env3.Message.ConversationId) && env1.Message.MessageNumber.Equals(env3.Message.MessageNumber));

            //Close the communicators for next test. 
            commSubSysPlayer.udpcomm.closeCommunicator();
            commSubSysPlayer.tcpcomm.closeCommunicator();
        }

        [TestMethod]
        public void PLayerJoinLobbyFullProtocolTest()
        {

            //Create a SubSystem with dummy factory. 
            dummyCoversationFactory dumConvoFact1 = new dummyCoversationFactory();
            LobbyAppState lobbyAppState = new LobbyAppState();
            SubSystem commSubSysLobby = new SubSystem(dumConvoFact1, lobbyAppState);

            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState playerAppState = new PlayerAppState(waitingRoom);
            SubSystem commSubSysPlayer = new SubSystem(dumConvoFact1, playerAppState);

            //Set the name of the player in the App State. 
            playerAppState.playerName = "Tester Testerson";

            //get the initial process id. This test will check that the conversation changes it. 
            int startingProcessId = commSubSysPlayer.ProcessID;

            //Create Initiator conversation
            PlayerJoinLobby PlayerJoinLobby = new PlayerJoinLobby();
            PlayerJoinLobby.SubSystem = commSubSysPlayer;
            PlayerJoinLobby.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            ////Create Responder Conversation
            RespondJoinLobby resJoinLobbyConvo = new RespondJoinLobby();
            resJoinLobbyConvo.SubSystem = commSubSysLobby;
            resJoinLobbyConvo.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222);

            //Start Initiator Conversation
            PlayerJoinLobby.Launch();

            //Wait for conversation to send message.
            Thread.Sleep(500);

            //Check for outgoing message.
            Assert.IsTrue(commSubSysPlayer.outQueue.Count >= 1);

            //Get message and put into responder conversation
            Envelope initiatorEnv;
            if (!commSubSysPlayer.outQueue.TryDequeue(out initiatorEnv)) Assert.Fail();
            resJoinLobbyConvo.IncomingEnvelope = initiatorEnv;
            resJoinLobbyConvo.ConversationId = initiatorEnv.Message.ConversationId;

            //Start the responder conversation
            resJoinLobbyConvo.Launch();

            //Wait for conversation to send message
            Thread.Sleep(500);

            //Check for outgoing message from responder conversation.
            Assert.IsTrue(commSubSysLobby.outQueue.Count >= 1);

            //Get responder reply and give to initiator. 
            Envelope responderEnv;
            if (!commSubSysLobby.outQueue.TryDequeue(out responderEnv)) Assert.Fail();
            PlayerJoinLobby.Process(responderEnv);

            //Check that the process Id was changed. Note: This is the 
            Assert.IsTrue(startingProcessId == commSubSysPlayer.ProcessID);
            Console.WriteLine(startingProcessId);
            Console.WriteLine(commSubSysLobby.ProcessID);

            //Check that the GM was added to the Lobby.
            Assert.IsTrue(lobbyAppState.Pls.Count >= 1);

            //Close the communicators for next test. 
            commSubSysLobby.udpcomm.closeCommunicator();
            commSubSysLobby.tcpcomm.closeCommunicator();
        }

    }
}
