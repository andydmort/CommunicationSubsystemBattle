using CommSubSys;
using Messages;
using System;
using log4net;

namespace GameManager
{
    public class Turns : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Turns));
        protected override Type[] ExceptedReplyType => new Type[] { typeof(AckMessage) };

        public bool myTurn = true; //Should be set to false before launch for start game turns conversation only for the player that is not starting. 

        protected override bool IsTcpConversation => true;

        GMAppState appState;

        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;

            appState = SubSystem.appState as GMAppState;
            Logger.Info("Result message created in turns convo");
            return new ResultMessage(appState.lastShotResult, appState.gameId, false, appState.end, this.myTurn, appState.LastX, appState.LastY, messageNumber, ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            var message = env.Message as AckMessage;
            // check message.getGORp()

            Logger.Info("Ack message received from turn message");
            Console.WriteLine("Ack message received from turn message");
        }

        protected override void ExecuteDetails()
        {
            base.ExecuteDetails();
            //Wont get here until after the ack has be returned. or failed to recieve. 
            if (appState.end)
            {
                Logger.Debug("Setting the game Ending event.");
                appState.gameEndingEvent.Set();
            }
        }

        //protected override void ExecuteDetails()
        //{
        //    base.ExecuteDetails();
        //    //    Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());

        //    //    Envelope recEnv = DoReliableRequestReply(null, ExceptedReplyType);
        //    //    if (recEnv == null)
        //    //    {
        //    //        Error = "No response received";
        //    //        Logger.Debug("No response received");
        //    //    }
        //    //    else
        //    //    {
        //    //        ProcessValidResponse(recEnv);
        //    //    }
        //}
    }
}
