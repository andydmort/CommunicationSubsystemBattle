using System;
using CommSubSys;
using Messages;
using log4net;

namespace GameManager
{
    public class RespondPassOff : ResponderConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondPassOff));

        protected override bool IsTcpConversation => false;

        protected override void ExecuteDetails()
        {
            Logger.Debug("Stating Execution of Respond Pass Off Conversation. ");
            //Respond to first Heart beat. 
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            //Reply to request
            this.DoReliableRespondToRequest(response);

            this.State = PossibleState.Successed;

        }

        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            if(env.Message.GetType() == typeof(PlayerIdMessage))
            {
                //TODO: Put info from PlayerIdMessage In AppState. 
                GMAppState gmAppState = (GMAppState)this.SubSystem.appState;
                PlayerIdMessage mess = (PlayerIdMessage)env.Message;

                gmAppState.p1PID = mess.getP1Id();
                gmAppState.p2PID = mess.getP2Id();

                gmAppState.P1EndPoint = SubSystem.ParseEPString(mess.getP1EndPoint());
                Logger.Debug($"Set the Player 1 endpoint to {mess.getP1EndPoint()}");

                gmAppState.P2EndPoint = SubSystem.ParseEPString(mess.getP2EndPoint());
                Logger.Debug($"Set the Player 2 endpoint to {mess.getP2EndPoint()}");


                gmAppState.p1Name = mess.getP1Name();
                gmAppState.p2Name = mess.getP2Name();

                gmAppState.gameId = mess.getGameId();

                //TODO: Not sure what to do with mess.getp1Wins and mess.getp2Wins. 

                return new AckMessage(99, 0, new Identifier(this.SubSystem.ProcessID, SubSystem.GetNextSeqNumber()), this.ConversationId); //TODO: Not sure what to do with the ID in Ack here. 
            }
            else
            {
                Logger.Error($"Did not receive a playerIdMessage, instead received a {env.Message.GetType()}");
            }

            return null;
        }

    }
}
