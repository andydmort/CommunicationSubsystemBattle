using System;
using CommSubSys;
using Messages;
using log4net;

namespace Player
{
    public class Board : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Board));
        public PlayerAppState playerAppState;
        protected override Type[] ExceptedReplyType => new Type[] { typeof(ErrorMessage), typeof(AckMessage)};

        protected override bool IsTcpConversation => true;

        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;

            playerAppState = SubSystem.appState as PlayerAppState;
            Logger.Info("Board message created in Board convo");
            return new BoardMessage(playerAppState.gameId, playerAppState.planningGrid, messageNumber, ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            if (env.Message.GetType() == typeof(AckMessage)) 
            {
                Logger.Info("Received an ack message in Board convo");
                var message = env.Message as AckMessage; // if this is received then we continue (we are handling this by not doing anything)
            }
                
            if (env.Message.GetType() == typeof(ErrorMessage))
            {
                Logger.Error("Received an error message in Board convo");
                var message = env.Message as ErrorMessage;
            }
        }

        //protected override void ExecuteDetails()
        //{
        //    base.ExecuteDetails();
        //    Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());

        //    Envelope recEnv = DoReliableRequestReply(null, ExceptedReplyType);
        //    if (recEnv == null)
        //    {
        //        Error = "No response received";
        //        Logger.Error("No response received in Board convo");
        //    }
        //    else
        //    {
        //        ProcessValidResponse(recEnv);
        //    }
        //}

    }
}
