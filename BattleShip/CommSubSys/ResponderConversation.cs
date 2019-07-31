using log4net;
using Messages;

namespace CommSubSys
{
    public abstract class ResponderConversation : Conversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ResponderConversation));

        /// <summary>
        /// For conversations started by an incoming message, this is the message
        /// </summary>
        public Envelope IncomingEnvelope { get; set; }

        protected override bool Initialize()
        {
            if (!base.Initialize()) return false;

            Identifier ConvId = IncomingEnvelope?.Message?.ConversationId;
            if (ConvId != null)
            {
                RemoteEndPoint = IncomingEnvelope?.From;
                this.ConversationId = ConvId;
                SubSystem.conversationFactory.dictionary.Add(this);
                State = PossibleState.Working;
            }
            else
                Error = $"Cannot initialize {GetType().Name} conversation because ConvId in incoming message is null";

            return (Error == null);
        }

        protected void DoReliableRespondToRequest(Envelope ResponseEnv)
        {
            Logger.Debug($"{this.GetType().Name}-{this.ConversationId}, Sending Response Envelope with message Type {ResponseEnv.Message.GetType()}");
            SubSystem.outQueue.Enqueue(ResponseEnv);
            //Start timeout
            int counter = 1;
            while (DuplicateMessageEvent.WaitOne(1000) && counter<10) //Wait if message comes in Redo the function. //TODO: talk about the max retry here. 
            {
                counter++;
                Logger.Debug($"{this.GetType().Name}-{this.ConversationId}, Resending Response Envelope with message Type {ResponseEnv.Message.GetType()}. Retry number {counter}");

                SubSystem.outQueue.Enqueue(ResponseEnv); // was causing a stack overflow with lots of duplicate messages. 
            }


        }

    }
}
