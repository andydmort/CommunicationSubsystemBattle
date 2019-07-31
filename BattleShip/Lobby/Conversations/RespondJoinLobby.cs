using CommSubSys;
using Messages;
using log4net;
using System;
using System.Net;
using System.Security.Cryptography; 

namespace Lobby
{
    public class RespondJoinLobby : ResponderConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondJoinLobby));

        protected override bool IsTcpConversation => false;

        protected override void ExecuteDetails()
        {
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            DoReliableRespondToRequest(response);
            //SubSystem.outQueue.Enqueue(response);
        }

        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            this.IncomingEnvelopes.Enqueue(env);
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());

            short ID = SubSystem.GetNextProcessId();
            string publicKey = "";
            //Adding Player to lobby
            if (env.Message.GetType() == typeof(PlayerJoinLobbyMessage))
            {
                PlayerJoinLobbyMessage playerJoinLobbyMessage = (PlayerJoinLobbyMessage)env.Message;
                ((LobbyAppState)SubSystem.appState).addPlayer(env.From, ID, playerJoinLobbyMessage.getPlayerName());
                publicKey = playerJoinLobbyMessage.getPublicKey();

                Console.WriteLine($"Recieved a PlayerJoinLobbyMessage from a player named {((PlayerJoinLobbyMessage)env.Message).getPlayerName()}");
                Logger.Debug($"Recieved a PlayerJoinLobbyMessage from a player named {((PlayerJoinLobbyMessage)env.Message).getPlayerName()}");
            }
            //Adding GM to lobby
            else if(env.Message.GetType() == typeof(GMJoinLobbyMessage))
            {
                GMJoinLobbyMessage gMJoinLobbyMessage = (GMJoinLobbyMessage)env.Message;
                IPEndPoint GMEP = new IPEndPoint(env.From.Address, gMJoinLobbyMessage.getPort());
                publicKey = gMJoinLobbyMessage.getPublicKey();

                ((LobbyAppState)SubSystem.appState).addGM(env.From,GMEP, ID);
                Console.WriteLine($"Recieved a GMJoinLobby message from a GM");
                Logger.Debug($"Recieved a GMJoinLobby message from a GM");
            }

            Tuple<byte[], byte[]> rm = ((LobbyAppState)SubSystem.appState).encryptSysKeys(publicKey);
            AssignIdMessage message = new AssignIdMessage(ID, rm.Item1, rm.Item2, messageNumber, this.ConversationId);
            // set the assignIdmessage attribute of encrypted key here
            // need to set an attribute in assignIdMessage that will 
            ((LobbyAppState)SubSystem.appState).gameIsReady();

            return message;
        }

        

    }
}
