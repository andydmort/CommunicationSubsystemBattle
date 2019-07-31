using CommSubSys;
using log4net;
using Messages;
using System;

namespace GameManager
{
    public class EndGame : InitiatorConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EndGame));
        protected override Type[] ExceptedReplyType => new Type[] { typeof(AckMessage) };

        protected override bool IsTcpConversation => false;

        public short Pid { get; set; }
        
        protected override Message CreateFirstMessage()
        {
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            ConversationId = messageNumber;

            return new EndGameResultMessage(Pid, ((GMAppState)SubSystem.appState).gameId, messageNumber, ConversationId);
        }

        protected override void ProcessValidResponse(Envelope env)
        {
            var message = env.Message as AckMessage;

            Logger.Debug("Ack message received from endgame message");
        }
    }
}
