using CommSubSys;
using Messages;
using log4net;

namespace Player
{
    public class RespondTurns : ResponderConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondTurns));
        private PlayerAppState playerAppState;
        protected override bool IsTcpConversation => true;

        protected override void ExecuteDetails()
        {
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            DoReliableRespondToRequest(response);

            if (!playerAppState.gameActive)
            {
                playerAppState.MoveToGame();
                playerAppState.gameActive = true;
            }
            Logger.Info($"Setting turn to {playerAppState.turn}");
        }

        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            this.IncomingEnvelopes.Enqueue(env);
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());

            if (env.Message.GetType() == typeof(ResultMessage))
            {
                Logger.Info("Received a result message in RespondTurns convo");
                playerAppState = SubSystem.appState as PlayerAppState;

                ResultMessage message = env.Message as ResultMessage;
                playerAppState.lastShotHit = message.getHit();
                playerAppState.turn = message.getMyTurn();
                playerAppState.end = message.getEnd();
                playerAppState.won = message.getWin();
                playerAppState.gameId = message.getGameId();
                playerAppState.lastX = message.getXcord();
                playerAppState.lastY = message.getYcord();
                playerAppState.SetTurn();
                playerAppState.ColorButtonResult(playerAppState.turn, playerAppState.lastX, playerAppState.lastY, playerAppState.lastShotHit); //Setting gameScreen according to shot. 
                
                return new AckMessage(playerAppState.gameId, 1, messageNumber, this.ConversationId);
            }
            else
            {
                Logger.Error("Received an error message in RespondTurns convo");
                return new ErrorMessage();
            }

        }
    }
}
