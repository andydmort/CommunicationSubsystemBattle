using CommSubSys;
using System.Net;
using log4net.Config;
using log4net;
using Messages;

namespace Lobby
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            ILog Logger = LogManager.GetLogger(typeof(Program));


            //string player1Name = "";
            LobbyAppState appstate = new LobbyAppState();
            ConversationFactory conFact = new ConversationFactory();
            SubSystem subSys = new SubSystem(conFact, appstate,new IPEndPoint(IPAddress.Any,2201),new IPEndPoint(IPAddress.Any,2201));
            subSys.start();

            // need to connect players to games
            // and bring them back after game is over
            while (true)
            {
                //appstate.gameIsReady();
                //WAIT till a game is ready. 
                if (!LobbyAppState.gameIsReadyEvent.WaitOne(10000)) {
                    Logger.Debug("Timeout for checking if game is ready has been reached going to try again.");
                    continue;
                }
               
               //Start the game. 
               short gameIDToStart;
               gameIDToStart = appstate.startGame();

               Logger.Debug("Started a game passoff conversation");
               
               //Create initiator conversations and set the conversation. 
               PassOff passOffLobby = new PassOff();
               passOffLobby.SubSystem = subSys;
               passOffLobby.ConversationId = new Identifier(passOffLobby.SubSystem.ProcessID, SubSystem.GetNextSeqNumber());
               passOffLobby.setEPs(gameIDToStart);
               passOffLobby.Launch();
                //Note: remote Endpoint set in constructor of passOffLobby.
                
            }

        }
    }
}
