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
    public class PassOffTest
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
        public void PassOffFullProtocol()
        {
            //Set up
            //Create the Players
            dummyConversationFactory dumConvoFactPlayer1 = new dummyConversationFactory();
            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState player1AppState = new PlayerAppState(waitingRoom);
            SubSystem subSystemPlayer1 = new SubSystem(dumConvoFactPlayer1, player1AppState);
            subSystemPlayer1.ProcessID = 3;

            dummyConversationFactory dumConvoFactPlayer2 = new dummyConversationFactory();
            WaitingRoom waitingRoom2 = new WaitingRoom();
            PlayerAppState player2AppState = new PlayerAppState(waitingRoom2);
            SubSystem subSystemPlayer2 = new SubSystem(dumConvoFactPlayer2, player2AppState);
            subSystemPlayer2.ProcessID = 4;

            //Create the GM
            dummyConversationFactory dumConvoFactGM = new dummyConversationFactory();
            GMAppState GMAppState = new GMAppState();
            SubSystem subSystemGM = new SubSystem(dumConvoFactGM, GMAppState);
            subSystemGM.ProcessID = 2;

            //Create the Lobby. 
            dummyConversationFactory dumConvoFactLobby = new dummyConversationFactory();
            LobbyAppState lobbyAppState = new LobbyAppState();
            SubSystem subSystemLobby = new SubSystem(dumConvoFactLobby, lobbyAppState);
            subSystemLobby.ProcessID = 1;

            //Add GM to app state. 
            lobbyAppState.addGM(new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222), new IPEndPoint(IPAddress.Parse("2.2.2.2"),2221), 2);
            //Add Players to app state. 
            lobbyAppState.addPlayer(new IPEndPoint(IPAddress.Parse("3.3.3.3"), 333), 3, "player1");
            lobbyAppState.addPlayer(new IPEndPoint(IPAddress.Parse("4.4.4.4"), 444), 4, "Player2");




            //Execution and Testing. 

            //Start the game in the app state. 
            short gameIDToStart;
            gameIDToStart = lobbyAppState.startGame();
            //make sure the game started
            Assert.IsTrue(gameIDToStart >0);

            //Create initiator conversations and set the conversation. 
            PassOff passOffLobby = new PassOff();
            passOffLobby.SubSystem = subSystemLobby;
            passOffLobby.ConversationId = new Identifier(passOffLobby.SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            passOffLobby.setEPs(gameIDToStart);
            //Note: remote Endpoint set in constructor of passOffLobby. 

            //Create Responder conversations
            GameManager.RespondPassOff GMRespondPassOff = new GameManager.RespondPassOff();
            GMRespondPassOff.SubSystem = subSystemGM;
            Player.RespondPassOff Pl1RespondPassOff = new Player.RespondPassOff();
            Pl1RespondPassOff.SubSystem = subSystemPlayer1;
            Player.RespondPassOff Pl2RespondPassOff = new Player.RespondPassOff();
            Pl2RespondPassOff.SubSystem = subSystemPlayer2;


            //Launch conversation. 
            passOffLobby.Launch();

            //Wait a weee bit. 
            Thread.Sleep(500);

           
            //------------------ PlayerID from Lobby to GM
            Envelope envPlayerIDFromLobbyToGM = new Envelope(null, null, null, false); //Empty env.
            //The loop below is need to make sure that we are pulling the correct message out of the out queue. Because of the reliablility its possible to get duplicate messages going out. 
            int attemptForCorrectMessage = 0;
            while (attemptForCorrectMessage < 6)
            {
                subSystemLobby.outQueue.waitIncomingEnv(3000);
                Thread.Sleep(500);
                if (!subSystemLobby.outQueue.TryDequeue(out envPlayerIDFromLobbyToGM) && envPlayerIDFromLobbyToGM?.Message.GetType() == typeof(PlayerIdMessage))
                {
                    continue; //no message was dequeued or wrong type was dequeued. 
                }
                if (envPlayerIDFromLobbyToGM.To.Equals(new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222)))
                {
                    break; //We got it. 
                }
                attemptForCorrectMessage++;
                if (attemptForCorrectMessage >= 6) Assert.Fail();
            }

            //Check the message. 
            Assert.AreEqual(new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2222), envPlayerIDFromLobbyToGM.To);

            Envelope tmp;

            //Clear the outgoint queue.
            while (subSystemGM.outQueue.TryDequeue(out tmp)) { }

            //Give the conversation the Message and launch responder conversation. 
            GMRespondPassOff.IncomingEnvelope = envPlayerIDFromLobbyToGM;
            GMRespondPassOff.recievedMessagesDict.TryAdd(envPlayerIDFromLobbyToGM.Message.MessageNumber.ToString(), envPlayerIDFromLobbyToGM);
            GMRespondPassOff.Launch();

            //Wait a lil bit. 
            Thread.Sleep(1000);

            //------------------ Ack trom GM to Lobby. 
            //Get the new env.
            if (!subSystemGM.outQueue.waitIncomingEnv(3000)) { Assert.Fail(); }
            Thread.Sleep(500);
            Envelope envFromGMToLobbyACK;
            if (!subSystemGM.outQueue.TryDequeue(out envFromGMToLobbyACK)) { Assert.Fail(); }
            //Check the message 
            Assert.IsTrue(envFromGMToLobbyACK.Message.GetType() == typeof(AckMessage));

            //Clear the outgoing queue of lobby. 
            while (subSystemLobby.outQueue.TryDequeue(out tmp)) { }

            //Give lobby the envGameIDFromLobbyToPlayer1.
            passOffLobby.Process(envFromGMToLobbyACK);

            //Wait a weee bit
            Thread.Sleep(300);
           


            //------------------ GameId from Lobby to Player1
            //Get the new env.
            Envelope envGameIDFromLobbyToPlayer1 = new Envelope(null, null, null, false); //Empty env.
            //The loop below is need to make sure that we are pulling the correct message out of the out queue. Because of the reliablility its possible to get duplicate messages going out. 
            attemptForCorrectMessage = 0;
            while (attemptForCorrectMessage < 6)
            {
                subSystemLobby.outQueue.waitIncomingEnv(3000);
                Thread.Sleep(500);
                if (!subSystemLobby.outQueue.TryDequeue(out envGameIDFromLobbyToPlayer1) && envGameIDFromLobbyToPlayer1.Message.GetType() == typeof(GameIdMessage))
                {
                    continue; //no message was dequeued, or wrong type of message was dequeued. 
                }
                if (envGameIDFromLobbyToPlayer1.To.Equals(new IPEndPoint(IPAddress.Parse("3.3.3.3"), 333)))
                {
                    break; //We got it. 
                }
                attemptForCorrectMessage++;
                if (attemptForCorrectMessage == 6) Assert.Fail();
            }

            //Check the message. 
            Assert.AreEqual(new IPEndPoint(IPAddress.Parse("3.3.3.3"), 333), envGameIDFromLobbyToPlayer1.To);


            //Clear the outgoint queue.
            
            while (subSystemPlayer1.outQueue.TryDequeue(out tmp)) { }

            //The the conversation the Message, and launch the conversation.
            Pl1RespondPassOff.IncomingEnvelope = envGameIDFromLobbyToPlayer1;
            Pl1RespondPassOff.recievedMessagesDict.TryAdd(envGameIDFromLobbyToPlayer1.Message.MessageNumber.ToString(), envGameIDFromLobbyToPlayer1);
            Pl1RespondPassOff.Launch();



            //------------------ Ack from Player1 to Lobby. 
            //Get the new env.
            if (!subSystemPlayer1.outQueue.waitIncomingEnv(3000)) { Assert.Fail(); }
            Thread.Sleep(500);
            Envelope envAckFromPl1ToLobby;//Empty env.
            if (!subSystemPlayer1.outQueue.TryDequeue(out envAckFromPl1ToLobby)) { Assert.Fail(); }

            //Clear the outgoing queue of lobby. 
            while (subSystemLobby.outQueue.TryDequeue(out tmp)) { }

            //Give lobby the envGameIDFromLobbyToPlayer1.
            passOffLobby.Process(envAckFromPl1ToLobby);



            //------------------ GameId from Lobby to Player2. 
            Envelope envGameIDFromLobbyToPlayer2 = new Envelope(null, null, null, false); //Empty env.
            //The loop below is need to make sure that we are pulling the correct message out of the out queue. Because of the reliablility its possible to get duplicate messages going out. 
            attemptForCorrectMessage = 0;
            while (attemptForCorrectMessage < 6)
            {
                subSystemLobby.outQueue.waitIncomingEnv(3000);
                Thread.Sleep(500);
                if (!subSystemLobby.outQueue.TryDequeue(out envGameIDFromLobbyToPlayer2) && envGameIDFromLobbyToPlayer2.Message.GetType() == typeof(GameIdMessage))
                {
                    continue; //no message was dequeued or wrong type was dequeued. 
                }
                if (envGameIDFromLobbyToPlayer2.To.Equals(new IPEndPoint(IPAddress.Parse("4.4.4.4"), 444)))
                {
                    break; //We got it. 
                }
                attemptForCorrectMessage++;
                if (attemptForCorrectMessage == 6) Assert.Fail();
            }

            //Check the message. 
            Assert.AreEqual(new IPEndPoint(IPAddress.Parse("4.4.4.4"), 444), envGameIDFromLobbyToPlayer2.To);

            //Clear the outgoint queue.
            while (subSystemPlayer2.outQueue.TryDequeue(out tmp)) { }

            //The the conversation the Message.
            Pl2RespondPassOff.IncomingEnvelope = envGameIDFromLobbyToPlayer2;
            Pl2RespondPassOff.recievedMessagesDict.TryAdd(envGameIDFromLobbyToPlayer2.Message.MessageNumber.ToString(), envGameIDFromLobbyToPlayer2);
            Pl2RespondPassOff.Launch();



            //------------------  Ack from Player2 to Lobby. 
            //Get the new env.
            if (!subSystemPlayer2.outQueue.waitIncomingEnv(3000)) { Assert.Fail(); }
            Thread.Sleep(500);
            Envelope envAckFromPl2ToLobby;//Empty env.
            if (!subSystemPlayer2.outQueue.TryDequeue(out envAckFromPl2ToLobby)) { Assert.Fail(); }

            //Clear the outgoing queue of lobby. 
            while (subSystemLobby.outQueue.TryDequeue(out tmp)) { }

            //Give lobby the envGameIDFromLobbyToPlayer1.
            passOffLobby.Process(envAckFromPl2ToLobby);

            //------------------ Make sure all the EndPoints are set correctly. 
            Assert.AreEqual(player1AppState.GMEndPoint, new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2221));
            Assert.AreEqual(player2AppState.GMEndPoint, new IPEndPoint(IPAddress.Parse("2.2.2.2"), 2221));

            Assert.AreEqual(GMAppState.P1EndPoint, new IPEndPoint(IPAddress.Parse("3.3.3.3"), 333));
            Assert.AreEqual(GMAppState.P2EndPoint, new IPEndPoint(IPAddress.Parse("4.4.4.4"), 444));
            

        }
    }
}
