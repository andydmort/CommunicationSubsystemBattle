using System;
using System.Collections.Generic;
using log4net;
using Messages;

namespace CommSubSys
{
    public abstract class ConversationFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConversationFactory));

        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();

        public ConversationDictionary dictionary = new ConversationDictionary();

        public SubSystem ManagingSubsystem { get; set; }
        public int DefaultMaxRetries { get; set; }
        public int DefaultTimeout { get; set; }

        /// <summary>
        /// Initialize
        /// 
        /// Concrete Factories should implement this method and that implementation should add program-specific
        /// Message-Type to Conversation-Type Mappings using the Add method
        /// </summary>
        public abstract void Initialize();

        protected void Add(Type messageType, Type conversationType)
        {
            if (messageType == null || !typeof(Message).IsAssignableFrom(messageType))
            {
                Logger.Error($"Invalid message type {messageType}");
                throw new ApplicationException("Invalid message type -- must be a specialization of Message");
            }

            if (conversationType == null || !typeof(ResponderConversation).IsAssignableFrom(conversationType))
            {
                Logger.Error($"Invalid conversation type {messageType}");
                throw new ApplicationException("Invalid conversation type -- must be a specialization of ResponderConversation");
            }

            if (!_typeMappings.ContainsKey(messageType))
                _typeMappings.Add(messageType, conversationType);
        }


        public virtual T CreateFromConversationType<T>() where T : InitiatorConversation, new()
        {
            var conversation = new T()
            {
                SubSystem = ManagingSubsystem,
                MaxRetries = DefaultMaxRetries,
                Timeout = DefaultTimeout,
            };

            return conversation;
        }


        public bool CanIncomingMessageStartConversation(Type messageType)
        {
            Logger.Debug("setting duplicate message even");
            return _typeMappings.ContainsKey(messageType);
        }

        public virtual Conversation CreateFromEnvelope(Envelope envelope)
        {
            Conversation conversation = null;
            var messageType = envelope?.Message?.GetType();

            if (messageType != null && _typeMappings.ContainsKey(messageType))
                conversation = CreateResponderConversation(_typeMappings[messageType], envelope);

            return conversation;
        }

        protected virtual ResponderConversation CreateResponderConversation(Type conversationType, Envelope envelope = null)
        {
            if (conversationType == null || envelope?.Message?.ConversationId == null)
            {
                Logger.WarnFormat("Cannot construct a Responder Conversation, conversationType={0} and convId={1}",
                    conversationType?.ToString() ?? "null",
                    envelope?.Message?.ConversationId?.ToString() ?? "null");
                return null;
            }

            ResponderConversation conversation = Activator.CreateInstance(conversationType) as ResponderConversation;
            if (conversation == null)
            {
                Logger.WarnFormat($"Cannot instantiate {conversationType}");
                return null;
            }

            //Logger.DebugFormat($"Create and started conversation of type",conversation.GetType().ToString());

            conversation.SubSystem = ManagingSubsystem;
            conversation.ConversationId = envelope.Message.ConversationId;
            conversation.IncomingEnvelope = envelope;
            conversation.IncomingEnvelopes.Enqueue(envelope);
            if(!conversation.recievedMessagesDict.TryAdd(envelope.Message.MessageNumber.ToString(), envelope))
            {
                Logger.Warn($"Unable to put message {envelope.Message.ConversationId}-{envelope.Message.MessageNumber} into recievedMessageDictionary while starting a resonder conversation.");
            }
            dictionary.Add(conversation);

            Logger.Debug($"Creating conversation with conversation id of {envelope.Message.ConversationId} and type of {conversationType}");
            conversation.Launch();

            return conversation;
        }
    }
}
