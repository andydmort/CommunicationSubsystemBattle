using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class GameScreen : Form
    {
        public PlayerAppState appState;
        public GameScreen()
        {
            InitializeComponent();


            //Initialize target grid
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PlanningButton b = new PlanningButton((short)i, (short)j);
                    b.Name = string.Format("TargetButton{0}{1}", i, j);
                    b.Text = string.Format("{0}{1}", i, j);
                    b.Dock = DockStyle.Fill;
                    b.BackColor = Color.LightSkyBlue;
                    TargetingTable.Controls.Add(b, i, j);
                    b.Click += (sender, e) => ShipTargetButton_Click(sender, e, b.xCoord, b.yCoord);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PlanningButton b = new PlanningButton((short)i, (short)j);
                    b.Name = string.Format("PlayerButton{0}{1}", i, j);
                    b.Text = string.Format("{0}{1}", i, j);
                    b.Dock = DockStyle.Fill;
                    b.Enabled = false;
                    b.BackColor = Color.LightSkyBlue;
                    PlayerShipTable.Controls.Add(b, i, j);
                }
            }
            FireButton.Enabled = false;
        }

        public void ResetColorButtons()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int ind = i * 10 + j;
                    PlayerShipTable.Controls[ind].BackColor = Color.LightSkyBlue;
                    TargetingTable.Controls[ind].BackColor = Color.LightSkyBlue;
                    TargetingTable.Controls[ind].Enabled = true;
                }
            }
        }

        public void SetTurn()
        {
            if (appState.turn)
            {
                StatusLabel.Text = "Your Turn";
            }
            else
            {
                StatusLabel.Text = appState.opponentName + "'s Turn";
            }
            if (appState.end && !appState.ended)
            {
                appState.ended = true;
                DialogResult result;
                if (appState.won)
                {
                    result = MessageBox.Show("You won! Play another game?", "Game Result", MessageBoxButtons.YesNo);
                }
                else
                {
                    result = MessageBox.Show(appState.opponentName + " won! Play another game?", "Game Result", MessageBoxButtons.YesNo);
                }
                if (result == DialogResult.Yes)
                {
                    appState.dangerousMoveToWaiting();
                    appState.Reset();
                }
                if(result == DialogResult.No)
                {
                    appState.subSystem.stop();
                    Environment.Exit(Environment.ExitCode);
                    Application.Exit();
                }
            }
        }

        public void LoadPlayerGrid()
        {
            for(int i = 0; i < 10; i++)
            {
                for(int j= 0; j < 10; j++)
                {
                    if (appState.planningGrid[i][j] == 1)
                    {
                        int ind = i * 10 + j;
                        PlayerShipTable.Controls[ind].BackColor = Color.Gray;
                    }
                }
            }
        }

        private void FireButton_Click(object sender, EventArgs e)
        {
            if(appState.shotCoordinates == null) { return; }
            FiringTargetLabel.Text = "(X, Y)";
            appState.CreateShotConversation();
            FireButton.Enabled = false;
            appState.shotCoordinates = null;
        }

        private void ShipTargetButton_Click(object sender, EventArgs e, short x, short y)
        {
            if (!appState.turn)
            {
                return;
            }
            int ind = x * 10 + y;
            appState.shotCoordinates = new Tuple<short, short>(x, y);
            FiringTargetLabel.Text = String.Format("({0}, {1})", x, y);
            FireButton.Enabled = true;
        }

        private void GameScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            appState.subSystem.stop();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }

        public void ColorButtonResult(bool turn, short x, short y, bool result) 
        {
            if (x == -1 || y == -1) return;
            int ind = x * 10 + y;
            if (turn)
            {
                if (!result)
                {
                    PlayerShipTable.Controls[ind].BackColor = Color.DodgerBlue;
                }
                else
                {
                    PlayerShipTable.Controls[ind].BackColor = Color.OrangeRed;
                }
                FireButton.Enabled = true;
            }
            else
            {
                if (!result)
                {
                    TargetingTable.Controls[ind].BackColor = Color.DodgerBlue;
                    TargetingTable.Controls[ind].Enabled = false;
                }
                else
                {
                    TargetingTable.Controls[ind].BackColor = Color.OrangeRed;
                    TargetingTable.Controls[ind].Enabled = false;
                }
            }
        }
    }
}
