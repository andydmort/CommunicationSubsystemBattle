using System;
using CommSubSys;
using Messages;
using log4net;

namespace Player
{
    public class RespondPassOff : ResponderConversation
    {
        PlayerAppState playerAppState;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondPassOff));

        protected override bool IsTcpConversation => false;

        protected override void ExecuteDetails()
        {
            playerAppState = (PlayerAppState)this.SubSystem.appState;
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);
            this.DoReliableRespondToRequest(response);

            playerAppState.MoveToPlanning();

            this.State = PossibleState.Successed;
        }

        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            if(env.Message.GetType() == typeof(GameIdMessage))
            {
                Logger.Debug("received a GameId message and now creating an Ack Message in ResponPassOff");
                //Get the appState
                GameIdMessage mess = (GameIdMessage)env.Message;

                //Set the players gameId. 
                playerAppState.gameId = mess.getGameId();
                Logger.Debug($"Set gameId to {playerAppState.gameId}");

                //Set the players GM EP.
                playerAppState.GMEndPoint = SubSystem.ParseEPString(mess.getGMEndpoint());
                Logger.Debug($"Set the GM endpoint to {mess.getGMEndpoint()}");

                //Set the Oponent name
                if (mess.getP1Name() == playerAppState.playerName)
                {
                    playerAppState.opponentName = mess.getP2Name();
                    Logger.Debug($"Set player 2 name to {mess.getP2Name()}");
                }
                else
                {
                    playerAppState.opponentName = mess.getP1Name();
                    Logger.Debug($"Set player 1 name to {mess.getP1Name()}");
                }

                return new AckMessage(999, 0, new Identifier(this.SubSystem.ProcessID, SubSystem.GetNextSeqNumber()), this.ConversationId); //TODO: ask group about Ack ID message here. 
            }
            else
            {
                Logger.Error($"Did not receive a playerIdMessage, instead received a {env.Message.GetType()}");
            }

            return null; //Failure to proccess env. 
        }

    }
}
