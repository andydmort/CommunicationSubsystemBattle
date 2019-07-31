using System.Threading;
using Messages;

using log4net;


namespace CommSubSys
{
    public class Dispatcher
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Dispatcher));

        public ConversationFactory conversationFactory { get; set; }
        EnvelopeQueue inQueue;
        bool running;
        Thread t;

        public Dispatcher(EnvelopeQueue inQueue)
        {
            this.inQueue = inQueue;
            running = false;            
        }

        public void Start()
        {
            if (running) return; //early breakout. 
            running = true;
            t = new Thread(new ThreadStart(DoStuff));
            t.IsBackground = false;
            Logger.Debug("Starting Dispatcher.");
            t.Start();
        }

        public void Stop()
        {
            if (!running) return;
            running = false;
            
            Logger.Debug("Stopping Dispatcher.");
        }

        public void DoStuff()
        {
            while (running)
            {

                if (!inQueue.IsEmpty)
                {
                    Envelope f;
                    if (inQueue.TryDequeue(out f))
                    {
                        ProcessEnvelope(f);
                    }
                    else
                    {
                        Logger.Error("Dispatcher failed to Dequeue Envelope from Incoming Queue.");                       
                    }
                }
                else
                {
                    //Thread.Sleep(20);
                    inQueue.waitIncomingEnv();
                }
            }
            t.Join();
        }

        public void ProcessEnvelope(Envelope env)
        {
            if (running)
            {
                Identifier id = env.Message.ConversationId;
                Conversation conv = conversationFactory.dictionary.GetConversation(id);
                if (conv != null)
                    conv.Process(env);
                else
                    conversationFactory.CreateFromEnvelope(env);
            }
            
        }
    }
}
