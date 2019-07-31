using System.Collections.Concurrent;
using System.Threading;

namespace CommSubSys
{
    public class EnvelopeQueue: ConcurrentQueue<Envelope>
    {
        /// <summary>
        /// This class is a thread safe queue with a incoming event that is set when an evelope gets enqueued. 
        /// </summary>
        private static AutoResetEvent incoming { get; set; } = new AutoResetEvent(false);

        //This method is meant to hide the Papa Enqueue method. 
        public new void Enqueue(Envelope env)
        {
            base.Enqueue(env);
            incoming.Set();
        }

        public AutoResetEvent getIncomingSignal()
        {
            return incoming;
        }

        public void setQueueSignal()
        {
            incoming.Set();
        }

        public void waitIncomingEnv()
        {
            incoming.WaitOne();
        }

        public bool waitIncomingEnv(int waitMiliseconds)
        {
            return incoming.WaitOne(waitMiliseconds);
        }

    }
}
