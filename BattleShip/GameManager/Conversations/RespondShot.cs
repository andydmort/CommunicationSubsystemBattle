using CommSubSys;
using Messages;
using System;
using log4net;

namespace GameManager
{
    public class RespondShot : ResponderConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondShot));
        private short GameId = 0;
        private GMAppState appState;
        protected override bool IsTcpConversation => true;

        protected override void ExecuteDetails()
        {
            appState = this.SubSystem.appState as GMAppState;

            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            DoReliableRespondToRequest(response);

            this.State = PossibleState.Successed;

            //Create Turns to other player. 
            Turns respondTurnsToShot = this.SubSystem.conversationFactory.CreateFromConversationType<Turns>();
            if (this.RemoteEndPoint.ToString() == appState.P1EndPoint.ToString())
            {
                respondTurnsToShot.RemoteEndPoint = appState.P2EndPoint;
            }
            else
            {
                respondTurnsToShot.RemoteEndPoint = appState.P1EndPoint;
            }
            respondTurnsToShot.Launch();
        }
        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            this.IncomingEnvelopes.Enqueue(env);
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());

            appState = SubSystem.appState as GMAppState;
            if (env.Message.GetType() == typeof(ShotMessage))
            {
                Logger.Info("Received a shot message in RespondShot convo");
                ShotMessage shotMessage = (ShotMessage)env.Message;
                GameId = shotMessage.getGameId();
                appState.isAHit(shotMessage.getXcord(), shotMessage.getYcord(), shotMessage.getPlayerId());
                appState.isAWin(shotMessage.getPlayerId());
                appState.P1turn = !appState.P1turn;
                appState.LastX = shotMessage.getXcord();
                appState.LastY = shotMessage.getYcord();

                ResultMessage resultMessage = new ResultMessage(appState.lastShotResult, GameId, appState.end, appState.end, false, appState.LastX, appState.LastY, messageNumber, this.ConversationId);
                return resultMessage;
            }
            else
            {
                Logger.Error("Did not receive a shot message in RespondShot convo");
                return new ErrorMessage(1, messageNumber, ConversationId);
            }
        }

    }
}
