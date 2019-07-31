using System.Collections.Concurrent;
using log4net;
using Messages;

namespace CommSubSys
{
    public class ConversationDictionary
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConversationDictionary));

        private readonly ConcurrentDictionary<string, Conversation> _activeConversation =
            new ConcurrentDictionary<string, Conversation>();
        
        public void Add(Conversation conversation)
        {
            string convID = conversation.ConversationId.ToString();
            if (!_activeConversation.ContainsKey(convID))
                _activeConversation.TryAdd(convID, conversation);       
        }

        public void Remove(Identifier conversationID)
        {
            Logger.Debug($"Remove Queue {conversationID}");
            string convID = conversationID.ToString();
            Conversation conversation;
            _activeConversation.TryRemove(convID, out conversation);
        }

        public Conversation GetConversation(Identifier conversationID)
        {
            string convID = conversationID.ToString();

            Conversation conversation;
            _activeConversation.TryGetValue(convID, out conversation);

            return conversation;
        }
    }
}
