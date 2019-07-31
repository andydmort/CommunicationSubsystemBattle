using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using log4net;
using Messages;

namespace CommSubSys
{
    public abstract class Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Conversation));

        /// This is the base abstraction for all conversations and as such, include a number of common properties for all conversations.
        /// Specifically, every conversation has reference to the communication subsystem, so it can access the Subsystem's UdpCommunicator
        /// and ConversationFactory.  It also keeps track of the Conversation Id given to it by the Conversation Factory.
        /// </summary>
        public ConcurrentQueue<Envelope> IncomingEnvelopes = new ConcurrentQueue<Envelope>();
        public ConcurrentDictionary<string,Envelope> recievedMessagesDict = new ConcurrentDictionary<string, Envelope>();
        protected readonly AutoResetEvent IncomingEvent = new AutoResetEvent(false);
        protected readonly AutoResetEvent DuplicateMessageEvent = new AutoResetEvent(false);
        public enum PossibleState
        {
            NotInitialized,
            Working,
            Failed,
            Successed
        };

        public PossibleState State { get; protected set; } = PossibleState.NotInitialized;

        public SubSystem SubSystem { get; set; }
        public IPEndPoint RemoteEndPoint  { get; set; }

        /// <summary>
        /// For conversations that will have a timeout, this is the timeout value in milliseconds
        /// </summary>
        public int Timeout { get; set; } = 3000;

        /// <summary>
        /// For conversation that can resend and retry the waiting for a reply, this is the maximum number of retries
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        public Identifier ConversationId { get; set; }
        
        //public Envelope FirstEnvelope { get; set; }

        public string Error { get; protected set; }

        public bool Done { get; protected set; }

        public void Launch(object context = null)
        {
            var result = ThreadPool.QueueUserWorkItem(Execute, context);
            Logger.DebugFormat("Launch of {0}, result = {1}", GetType().Name, result);
        }

        /// <summary>
        /// Execute
        /// 
        /// This method executes a conversation.  This is used by Launch, but can be called directly to run a conversation on the
        /// current thread.
        /// 
        /// This method is a template method that calls two abstract or virtual methods, namely Initialize and ExecuteDetails.
        /// Concrete specializations (or intermediate base classes) need to implement these methods.
        /// </summary>
        /// <param name="context"></param>
        public void Execute(object context = null)
        {
            if (Initialize())
                ExecuteDetails();

            if (string.IsNullOrEmpty(Error))
                State = PossibleState.Successed;
            else
            {
                State = PossibleState.Failed;
                Logger.Warn(Error);
            }

            if (SubSystem.conversationFactory.dictionary.GetConversation(ConversationId) != null)
                SubSystem.conversationFactory.dictionary.Remove(ConversationId);
        }

        public void Process(Envelope env)
        {
            //if (env?.Message == null || env.From == null) return;
            if (env?.Message == null) return;
            if (!isDuplicateMessage(env)) //This will throw away duplicate messages.
            {
                IncomingEnvelopes.Enqueue(env);
                if (!recievedMessagesDict.TryAdd(env.Message.MessageNumber.ToString(), env))
                {
                    Logger.Warn($"Unable to add message number {env.Message.MessageNumber} to received Messages Dictionary.");
                }

                Logger.Debug($"Setting the incoming event. New Message {env.Message.ConversationId}-{env.Message.MessageNumber}");
                IncomingEvent.Set();
            }
            else
            {
                Logger.Debug("Setting duplicate message event");
                DuplicateMessageEvent.Set();
            }
        }

        //This only checks if the message comming in is the same as the one before it. 
        public bool isDuplicateMessage(Envelope env)
        {
            Envelope lastEnv = null;
            //if(IncomingEnvelopes.TryPeek(out lastEnv))
            //{
            //if(lastEnv.Message.MessageNumber.Equals( env.Message.MessageNumber))
            //{
                    Envelope tempEnv; 
                    if(this.recievedMessagesDict.TryGetValue(env.Message.MessageNumber.ToString(), out tempEnv))
                    {
                        Logger.Debug($"Conversation {GetType().Name} found a duplicate message of type {env.Message.GetType()} with Identifier {env.Message.ConversationId}-{env.Message.MessageNumber}.");

                         return true;
                    }
                //}
            //}
            return false;
        }



        /// <summary>
        /// Initializes all computated state information for the conversation.  It should be called after the public
        /// properties for the conversation are setup.  For example, it should be called after the ConvId is set and
        /// if the conversation is a Initiator, after the RemoteEndPoint is set.  A specialization may override this
        /// method as needed, but the override should have as its first line the following:
        /// 
        ///     if (!base.Initialize()) return false;
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool Initialize()
        {
            State = PossibleState.Working;
            return true;
        }

        /// <summary>
        /// ExecuteDetails
        /// 
        /// This is called by Execute as one of the steps in that template method.  Concrete specialization (or intermediate
        /// classes) need to implement this method.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void ExecuteDetails();

        protected abstract bool IsTcpConversation { get; }
    }
}
