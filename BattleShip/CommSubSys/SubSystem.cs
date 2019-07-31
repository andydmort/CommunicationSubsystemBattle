using System.Diagnostics;
using System.Net;
using log4net;


namespace CommSubSys
{
    public class SubSystem
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SubSystem));

        public Dispatcher dispatcher;
        public EnvelopeQueue inQueue = new EnvelopeQueue();
        public EnvelopeQueue outQueue = new EnvelopeQueue();
        public TcpCommunicator tcpcomm;
        public UdpCommunicator udpcomm;
        public ConversationFactory conversationFactory { get; set; }
        public AppState appState;

        #region Message number. Used by conversations to create messages
        
        public short ProcessID { get; set; }            // First number in message number (ProcessID,SequenceNumber)
        public static object Integer { get; private set; }

        private static short _nextSeqNumber = 0;        // Initialize to 0, which means it will start with message #1
        private static readonly object seqNumLock = new object();

        public static short GetNextSeqNumber()
        {
            lock (seqNumLock)
            {
                if (_nextSeqNumber == short.MaxValue)
                    _nextSeqNumber = 0;
                ++_nextSeqNumber;
            }
            return _nextSeqNumber;
        }
        #endregion

        #region Process ID. Used by Lobby only
        private static short _nextProcessId = 1;        // Initialize to 1, which means it will start with process id #2. Lobby will be process 1
        private static readonly object processIdLock = new object();

        public SubSystem(ConversationFactory convoFactory, AppState appState)
        {
            ProcessID = (short)Process.GetCurrentProcess().Id;
            this.appState = appState;
            this.conversationFactory = convoFactory;
            convoFactory.ManagingSubsystem = this; //Set the convsersation factories SubSystem to this class. 
            dispatcher = new Dispatcher(inQueue);
            dispatcher.conversationFactory = convoFactory;
            tcpcomm = new TcpCommunicator( inQueue,  outQueue, new IPEndPoint(IPAddress.Any,0));
            udpcomm = new UdpCommunicator( inQueue,  outQueue, new IPEndPoint(IPAddress.Any,0)); //Set port for udp recieving.
            
            this.conversationFactory.Initialize();
        }

        public SubSystem(ConversationFactory convoFactory, AppState appState, IPEndPoint TCPEP, IPEndPoint UDPEP)
        {
            ProcessID = 0;
            this.appState = appState;
            this.conversationFactory = convoFactory;
            convoFactory.ManagingSubsystem = this; //Set the convsersation factories SubSystem to this class. 
            dispatcher = new Dispatcher(inQueue);
            dispatcher.conversationFactory = convoFactory;
            tcpcomm = new TcpCommunicator(inQueue, outQueue, TCPEP);
            udpcomm = new UdpCommunicator(inQueue, outQueue, UDPEP); //Set port for udp recieving.

            this.conversationFactory.Initialize();
        }

        public void start()
        {
            Logger.Debug("Starting Communication SubSystem.");
            tcpcomm.startThreads();
            udpcomm.startThreads();
            dispatcher.Start();

        }

        public void stop()
        {
            Logger.Debug("Stopping Communication subsystem.");
            tcpcomm.closeCommunicator();
            udpcomm.closeCommunicator();
            dispatcher.Stop();
        }

        public static short GetNextProcessId()
        {
            lock (processIdLock)
            {
                if (_nextProcessId == short.MaxValue)
                    _nextProcessId = 1;
                ++_nextProcessId;
            }
            return _nextProcessId;
        }

        #endregion

        public static IPEndPoint ParseEPString(string hostnameAndPort)
        {
            if (string.IsNullOrWhiteSpace(hostnameAndPort)) return null;

            IPEndPoint result = null;
            var tmp = hostnameAndPort.Split(':');
            int port;
            if (tmp.Length == 2 && !string.IsNullOrWhiteSpace(tmp[0]) && !string.IsNullOrWhiteSpace(tmp[1]) && System.Int32.TryParse(tmp[1], out port))
                result = new IPEndPoint(IPAddress.Parse(tmp[0]), port);

            return result;
        }

    }
}
