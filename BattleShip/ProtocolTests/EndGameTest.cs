using System;
using System.Net;
using System.Threading;
using CommSubSys;
using GameManager;
using Lobby;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests
{
    [TestClass]
    public class EndGameTest
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
        public void EndGameRetries()
        {

            dummyConversationFactory dumConvoFact = new dummyConversationFactory();
            GMAppState gmAppState = new GMAppState();
            SubSystem commSubSys = new SubSystem(dumConvoFact, gmAppState);

            EndGame endGame = new EndGame();
            endGame.SubSystem = commSubSys;
            endGame.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111);

            endGame.Launch();

            Thread.Sleep(1000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 1);

            Thread.Sleep(2000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 2);

            Thread.Sleep(2000);

            Assert.IsTrue(commSubSys.outQueue.Count >= 3);

            //Make sure those messages are the same. 
            Envelope env1;
            if (!commSubSys.outQueue.TryDequeue(out env1))
            {
                Assert.Fail();
            }
            Envelope env2;
            if (!commSubSys.outQueue.TryDequeue(out env2))
            {
                Assert.Fail();
            }
            Envelope env3;
            if (!commSubSys.outQueue.TryDequeue(out env3))
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
        public void EndGameFull()
        {
            dummyConversationFactory lobbyDumConvoFac = new dummyConversationFactory();
            LobbyAppState lobbyAppState = new LobbyAppState();
            SubSystem commSubSysLobby = new SubSystem(lobbyDumConvoFac, lobbyAppState);

            dummyConversationFactory gMDumConvoFac = new dummyConversationFactory();
            GMAppState gMAppState = new GMAppState();
            SubSystem comSubSysGM = new SubSystem(gMDumConvoFac, gMAppState);

            //Setting a running game in the lobby appState
            lobbyAppState.addGM(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111), new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222), 5);
            lobbyAppState.addPlayer(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111), 1, "name");
            lobbyAppState.addPlayer(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 1111), 2, "something");
            lobbyAppState.startGame(); //pulls the players and GM into game. 
            Assert.IsTrue(lobbyAppState.GamesBeingPlayed.Count == 1);

            //Setting the GMID 
            gMAppState.gameId = 5;

            //Create the initators conversation
            EndGame endGame = new EndGame();
            endGame.SubSystem = comSubSysGM;
            endGame.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 111);

            //Create the responder
            RespondEndGame resEndGame = new RespondEndGame();
            resEndGame.SubSystem = commSubSysLobby;
            resEndGame.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("2.2.2.2"), 222);

            //Start initiator conversatin
            endGame.Launch();

            //wait for conversation to send message
            Thread.Sleep(1000);

            //Check for outgoing message.
            Assert.IsTrue(comSubSysGM.outQueue.Count >= 1);

            //Get the message from initiator to responder. 
            Envelope initiatorEnv;
            if (!comSubSysGM.outQueue.TryDequeue(out initiatorEnv)) Assert.Fail();
            resEndGame.IncomingEnvelope = initiatorEnv;
            resEndGame.ConversationId = initiatorEnv.Message.ConversationId;

            //start the responder conversation
            resEndGame.Launch();

            //wait a bit
            Thread.Sleep(1000);

            //Check of a responce from the conversation
            Assert.IsTrue(commSubSysLobby.outQueue.Count >= 1);

            //Get the responderenv
            Envelope responderEnv;
            if (!commSubSysLobby.outQueue.TryDequeue(out responderEnv)) Assert.Fail();

            Assert.IsTrue(resEndGame.ConversationId == endGame.ConversationId);
            Assert.IsTrue(lobbyAppState.GamesBeingPlayed.Count == 0);

        }
    }
}
