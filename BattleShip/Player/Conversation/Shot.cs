using CommSubSys;
using Messages;
using System;
using log4net;

namespace Player
{
    public class Shot : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Shot));
        public PlayerAppState appState;
        protected override Type[] ExceptedReplyType => new Type[] { typeof(ErrorMessage), typeof(ResultMessage)};

        protected override bool IsTcpConversation => true;

        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;

            appState = SubSystem.appState as PlayerAppState;
            Logger.Info("Shot message created in Shot convo");
            return new ShotMessage(appState.shotCoordinates.Item1, appState.shotCoordinates.Item2, appState.gameId, appState.playerID, messageNumber, ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            if (env.Message.GetType() == typeof(ResultMessage))
            {
                Logger.Info("Received a result message in Shot convo");
                var message = env.Message as ResultMessage;
                appState.turn = message.getMyTurn();
                appState.end = message.getEnd();
                appState.won = message.getWin();
                appState.lastShotHit = message.getHit();
                appState.lastX = message.getXcord();
                appState.lastY = message.getYcord();
                appState.SetTurn();
                appState.ColorButtonResult(appState.turn, appState.lastX, appState.lastY, appState.lastShotHit); //Setting gameScreen according to shot. 
            }
            if (env.Message.GetType() == typeof(ErrorMessage))
            {
                Logger.Error("Received an error message in Shot convo");
                var message = env.Message as ErrorMessage;
            }
        }

        protected override void ExecuteDetails()
        {
            base.ExecuteDetails();
        }
    }
}
