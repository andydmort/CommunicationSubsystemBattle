using System.Net;
using CommSubSys;
using System.Threading;
using log4net.Config;
using log4net;

namespace GameManager
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            ILog Logger = LogManager.GetLogger(typeof(Conversation));

            GMAppState appState = new GMAppState();
            IPEndPoint lobbyEndPoint = new IPEndPoint(IPAddress.Parse("54.159.115.195"), 2201);

            ConversationFactory conFact = new ConversationFactory(); //Create conversation factory
            IPEndPoint tcpEP = new IPEndPoint(IPAddress.Any, 2005);
            IPEndPoint updEP = new IPEndPoint(IPAddress.Any, 2006);
            SubSystem subSys = new SubSystem(conFact, appState,tcpEP, updEP); //Create Communication sub system. 
            subSys.start(); //Start the subsystem. 

            short count = 2;
            while (count-- > 0)
            {
                GMJoinLobby conversation = conFact.CreateFromConversationType<GMJoinLobby>();
                conversation.RemoteEndPoint = lobbyEndPoint; //Set to remote aws machine. 
                conversation.Launch();

                while (appState.P1grid == null || appState.P2grid == null)
                {
                    if (appState.P1grid != null)
                        Logger.Info("waiting for player 2's grid");
                    else
                        Logger.Info("waiting for both grids");
                    Thread.Sleep(3000);
                }

                Turns conversationTurn1 = conFact.CreateFromConversationType<Turns>();
                conversationTurn1.RemoteEndPoint = appState.P1EndPoint;
                Turns conversationTurn2 = conFact.CreateFromConversationType<Turns>();
                conversationTurn2.myTurn = false; //This makes sure pl2 know its not his turn. 
                conversationTurn2.RemoteEndPoint = appState.P2EndPoint;
                conversationTurn1.Launch();
                conversationTurn2.Launch();

                Logger.Info("Game in progress");
                while (!appState.gameEndingEvent.WaitOne(30) || appState.end == false)
                    Thread.Sleep(1000);
                Logger.Info("Game ending");

                EndGame conversationEndGame = conFact.CreateFromConversationType<EndGame>();
                conversationEndGame.RemoteEndPoint = lobbyEndPoint;
                conversationEndGame.Pid = appState.P1turn ? appState.p1PID : appState.p2PID;
                conversationEndGame.Launch();

                string p1result = appState.P1turn ? "WIN" : "LOSS";
                string p2result = !appState.P1turn ? "WIN" : "LOSS";

                Logger.Info($"Game over: Player 1 - {p1result}, Player 2 - {p2result}");

                appState.resetState();
            }
        }
    }
}
