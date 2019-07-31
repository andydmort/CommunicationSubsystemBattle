using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Player
{
    public partial class WaitingRoom : Form
    {
        public PlayerAppState appState;
        public PlanningScreen planningScreen;
        public GameScreen gameScreen;
        public WaitingRoom()
        {
            planningScreen = new PlanningScreen();
            planningScreen.Show();
            planningScreen.Hide();
            gameScreen = new GameScreen();
            gameScreen.Show();
            gameScreen.Hide();
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            string port = PortTextBox.Text;
            string address = IPAddressTextBox.Text;
            int portnum;
            IPAddress IP;

            if (PlayerNameTextBox.Text.Count() == 0)
            {
                WaitingLabel.Text = "Please enter a name.";
                WaitingLabel.Visible = true;
                return;
            }
            appState.playerName = PlayerNameTextBox.Text;
            try
            {
                bool validport = int.TryParse(port, out portnum);
                bool validIP = IPAddress.TryParse(address, out IP);
                if (!validport)
                {
                    if (!validIP)
                    {
                        WaitingLabel.Text = "You have entered an invalid IP address.\n Please enter a valid IP address.";
                        WaitingLabel.Visible = true;
                        return;
                    }
                    WaitingLabel.Text = "You have entered an invalid port value.\n Please enter a valid port number.";
                    WaitingLabel.Visible = true;
                    return;
                }
                appState.lobbyEndPoint = new IPEndPoint(IP, portnum);
            }
            catch(Exception ex)
            {
                WaitingLabel.Text = ex.Message;
                WaitingLabel.Visible = true;
            }
            appState.CreatePlayerJoinConversation();
            WaitingLabel.Text = "Connecting to lobby...";
            WaitingLabel.Visible = true;
        }

        public void EnterWaiting()
        {
            WaitingLabel.Invoke((MethodInvoker)delegate
            {
                WaitingLabel.Text = "Connected. Waiting for game...";
            });
        }

        private void WaitingRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            appState.subSystem.stop();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }

        public void resetText()
        {
            WaitingLabel.Text = "";
        }
    }
}
