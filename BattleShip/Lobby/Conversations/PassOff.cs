using System;
using CommSubSys;
using Messages;
using System.Net;
using log4net;
using Lobby.AppLogic;

namespace Lobby
{
    public class PassOff : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PassOff));

        protected override Type[] ExceptedReplyType => new Type[] { typeof(AckMessage) };

        protected override bool IsTcpConversation => false;


        public short gameID; //this must be set before starting this conversation. 
        //These endpoint must be set before starting this conversation. 
        public IPEndPoint Player1EP;
        public IPEndPoint Player2EP;
        //The code below ties the Remote endpoint to the GameManagerEP. 
        public IPEndPoint GameManagerEP {
            get { return this.RemoteEndPoint; }
            set { this.RemoteEndPoint = value; }
        }

        //This set the EPs for the conversation. 
        public void setEPs(short gameID)
        {
            this.gameID = gameID;
            LobbyGame game;
            if (!((LobbyAppState)SubSystem.appState).GamesBeingPlayed.TryGetValue(gameID, out game))
            {
                Logger.Error("Unable to Create PassOff conversation because pass in game ID not found in App state dictionary.");
            }
            this.Player1EP = game.Pl1.EP;
            this.Player2EP = game.Pl2.EP;
            this.GameManagerEP = game.GM.UDPEP;
            Logger.Debug("End points were set");
        }

        private short HeartBeatID = 0; //used to know what acknowledge is comming in. 

        protected override Message CreateFirstMessage()
        {
            LobbyGame game = ((LobbyAppState)SubSystem.appState).getGame(this.gameID);
            Logger.Debug($"Creating first message of playerID");
            return new PlayerIdMessage(this.gameID, game.Pl1.Id, game.Pl2.Id, game.Pl1.EP.ToString(), game.Pl2.EP.ToString(), game.Pl1.name, game.Pl2.name, false, false, false, false, new Identifier(this.SubSystem.ProcessID, SubSystem.GetNextSeqNumber()), this.ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            if (env.Message.GetType() == typeof(AckMessage))
            {
                var message = env.Message as AckMessage; // if this is received then we continue (we are handling this by not doing anything)
                Logger.Debug($"Received a Ack message from {env.Message.MessageNumber.getPid()} messageID {env.Message.ConversationId}-{env.Message.MessageNumber}");
            }
            else
            {
                Logger.Error($"Did not receive a Ack Message, instead received a {env.Message.GetType()}");
            }
        }

        protected override void ExecuteDetails()
        {
            //This handles the Heartbeat and ack responce from GM.
            base.ExecuteDetails();

            //Get the game and passoff data
            LobbyAppState lobAppState = (LobbyAppState)SubSystem.appState;
            LobbyGame game = lobAppState.getGame(this.gameID);

            //Starting Official Passoff. 
            //Handling GameId to player1
            Envelope envToPlayer1 = new Envelope(new GameIdMessage(this.gameID,game.Pl1.Id,game.Pl2.Id,game.GM.TCPEP.ToString(),game.Pl1.name,game.Pl2.name,false,false,false,false, new Identifier(this.SubSystem.ProcessID, SubSystem.GetNextSeqNumber()), this.ConversationId), null, Player1EP, IsTcpConversation);
            Envelope recievedFromPlayer1 = DoReliableRequestReply(envToPlayer1, this.ExceptedReplyType);
            if (recievedFromPlayer1 == null)
            {
                Logger.Error($"{this.GetType()}-{this.ConversationId}, Failed to recieve Env from player1. Ending Conversation Early.");
                return; //ending conversation early. 
            }
            this.ProcessValidResponse(recievedFromPlayer1);

            //Handling GameId to player2
            Envelope envToPlayer2 = new Envelope(new GameIdMessage(this.gameID, game.Pl1.Id, game.Pl2.Id, game.GM.TCPEP.ToString(), game.Pl1.name, game.Pl2.name, false, false, false, false, new Identifier(this.SubSystem.ProcessID, SubSystem.GetNextSeqNumber()), this.ConversationId), null, Player2EP, IsTcpConversation);
            Envelope recievedFromPlayer2 = DoReliableRequestReply(envToPlayer2, this.ExceptedReplyType);
            if (recievedFromPlayer2 == null)
            {
                Logger.Error($"{this.GetType()}-{this.ConversationId}, Failed to recieve Env from player2. Ending Conversation Early.");
                return; //ending conversation early. 
            }
            this.ProcessValidResponse(recievedFromPlayer2);

            //Conversation ends in success. 
            this.State = PossibleState.Successed;
            Logger.Info("Pass off ended in success!");
        }



        private short getNextHeartBeatID()
        {
            HeartBeatID++;
            return HeartBeatID;
        }
    }
}
