using log4net;
using Messages;
using System;
using System.Linq;

namespace CommSubSys
{
    public abstract class InitiatorConversation : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InitiatorConversation));

        protected Envelope FirstEnvelope { get; set; }

        /// <summary>
        /// Initialize
        /// 
        /// This method is called by Execute and is itself a template method, meaning that it calls
        /// at least one abstract or virtual method that has to be implemented by concrete specializations.
        /// In this case, the CreateFirstMessage method is the method that concrete specialization have
        /// to implement.
        /// </summary>
        /// <returns></returns>
        protected override bool Initialize()
        {
            if (!base.Initialize()) return false;

            if (RemoteEndPoint == null)
            {
                Logger.Error("No remote end point set for an initiator conversation");
                return false;
            }

            FirstEnvelope = null;

            var msg = CreateFirstMessage();
            if (msg == null) return false;

            SubSystem.conversationFactory.dictionary.Add(this);

            FirstEnvelope = new Envelope(msg, null, RemoteEndPoint, IsTcpConversation);
            
            return true;
        }

        protected abstract Message CreateFirstMessage();

        /// <summary>
        /// Execute Details
        /// 
        /// This method is called by Execute and is itself a template method, meaning
        /// that it calls some abstract method that have be implemented about specialization
        /// of this class.
        /// </summary>
        /// <param name="context"></param>
        protected override void ExecuteDetails()
        {
            State = PossibleState.Working;
            var env = DoReliableRequestReply(FirstEnvelope, ExceptedReplyType); // this is some ninja jujitsu inheritance
            Logger.Debug("Back from DoReliableRequestReply with ");

            if (env == null)
            {
                Error = "No response received";
            }
            else
            {
                ProcessValidResponse(env);
            }
        }
        // ExecuteDetails can continue in child class

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="messageTypes"> the expected type to receive from the request </param>
        /// <returns></returns>
        protected Envelope DoReliableRequestReply(Envelope env, Type[] messageTypes)
        {
            Logger.Debug($"{this.GetType().Name}-{this.ConversationId}, Sending Envelope with message Type {env.Message.GetType()}");
            SubSystem.outQueue.Enqueue(env); 

            //Start timeout for message.
            int maxAttemps = 25;
            int attempts = 1;
            while (!IncomingEvent.WaitOne(1000) && attempts < maxAttemps )
            {
                Logger.Debug($"{this.GetType().Name}-{this.ConversationId}, Sending Envelope with message Type {env.Message.GetType()} - Retry number {attempts}");
                SubSystem.outQueue.Enqueue(env);
                attempts++;
            }

            Envelope receivedEnv = null;
            if (IncomingEnvelopes.TryDequeue(out receivedEnv))
            {
                if (!messageTypes.Contains(receivedEnv.Message.GetType()))
                {
                    //We recieved the wrong message, or message we not in queue.
                    Logger.DebugFormat("{0}-{1}, Recieved wrong message.", this.GetType().Name, this.ConversationId);
                    receivedEnv = null; //Makes sure failure is null. 
                    this.State = PossibleState.Failed;
                }
            }
            else
            {
                Logger.DebugFormat("{0}-{1}, Didn't receive a message in Request Reply.", this.GetType().Name, this.ConversationId);
            }
            return receivedEnv; //Returns null on a failure. 
        }

        protected abstract Type[] ExceptedReplyType { get; }

        protected abstract void ProcessValidResponse(Envelope env);

    }
}
