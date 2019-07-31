using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommSubSys;

using log4net.Config;

namespace Player
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            WaitingRoom waitingRoom = new WaitingRoom();
            PlayerAppState appState = new PlayerAppState(waitingRoom);
            ConversationFactory conFact = new ConversationFactory(); //Create conversation factory
            SubSystem subsystem = new SubSystem(conFact, appState);
            appState.subSystem = subsystem;
            subsystem.start();
            Application.Run(waitingRoom);
       
            //Conversation conversation2   = conFact.CreateFromConversationType<Board>();
            //conversation2.RemoteEndPoint = appState.GMEndPoint;
            //conversation2.Launch();
            //Needs ConversationFactory implementation
            //ConversationFactory conFact = new ConversationFactory(subsystem);
        }
    }
}
