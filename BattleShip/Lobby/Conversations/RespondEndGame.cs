using CommSubSys;
using Messages;

namespace Lobby
{
    public class RespondEndGame : ResponderConversation
    {
        protected override bool IsTcpConversation => false;

        protected override void ExecuteDetails()
        {
            var msg = ProcessIncomingEnvelope(IncomingEnvelope);
            Envelope response = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);

            DoReliableRespondToRequest(response);
        }

        protected Message ProcessIncomingEnvelope(Envelope env)
        {
            IncomingEnvelopes.Enqueue(env);
            Identifier messageNumber = new Identifier(SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
            
            if (env.Message.GetType() == typeof(EndGameResultMessage))
            {
                ((LobbyAppState)SubSystem.appState).removeGame(((EndGameResultMessage)env.Message).getGameId());
                return new AckMessage(0, 0, messageNumber, ConversationId);
            }
            return new ErrorMessage(0, messageNumber, ConversationId);
        }
    }
}
