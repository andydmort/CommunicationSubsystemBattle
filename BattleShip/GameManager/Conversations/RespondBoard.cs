using CommSubSys;
using Messages;
using log4net;
using System;

namespace GameManager
{
    public class RespondBoard : ResponderConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RespondBoard));
        private static readonly object boardAccessLock = new object();
        protected override bool IsTcpConversation => true;

        protected override void ExecuteDetails()
        {
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            DoReliableRespondToRequest(response);
        }
        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            this.IncomingEnvelopes.Enqueue(env);
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());


            if (env.Message.GetType() == typeof(BoardMessage))
            {
                Logger.Info("Received a board message in RespondBoard convo");
                BoardMessage boardMessage = (BoardMessage)env.Message;
                GMAppState appState = SubSystem.appState as GMAppState;
                lock (boardAccessLock)
                {
                    if (appState.P1grid == null) //Board message maybe should include player id. That way we dont have to check the endpoints. 
                    {
                        appState.P1grid = boardMessage.getGrid();
                        appState.p1PID = boardMessage.MessageNumber.Pid;
                        appState.P1EndPoint = env.From;
                        Logger.Info("set player 1's grid in GM");
                    }
                    else
                    {
                        appState.P2grid = boardMessage.getGrid();
                        appState.p2PID  = boardMessage.MessageNumber.Pid;
                        appState.P2EndPoint = env.From;
                        Logger.Info("set player 2's grid in GM");
                    }
                }
                return new AckMessage(boardMessage.getGameId(), 0, messageNumber, this.ConversationId);
            }
            else
            {
                Logger.Error("Did not receive a board message in RespondBoard convo");
                return new ErrorMessage(2, messageNumber, ConversationId);
            }
        }
    }
}
