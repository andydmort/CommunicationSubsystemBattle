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
    public partial class PlanningScreen : Form
    {
        public PlayerAppState appState;
        PlanningShip currentShip; //Make planningShip object
        List<PlanningShip> shipList;
        List<RadioButton> shipRadioList;
        List<Label> shipCheckList;

        public PlanningScreen()
        {
            InitializeComponent();

            InitializeStates();
        }

        public void InitializeStates()
        {
            shipList = new List<PlanningShip>
            {
                new PlanningShip(5),
                new PlanningShip(4),
                new PlanningShip(3),
                new PlanningShip(3),
                new PlanningShip(2)
            };

            currentShip = shipList[0];


            shipRadioList = new List<RadioButton>
            {
                CarrierRadio,
                BattleshipRadio,
                CruiserRadio,
                SubRadio,
                DestroyerRadio
            };

            CarrierRadio.Checked = true;

            shipCheckList = new List<Label>
            {
                CarrierCheck,
                BattleshipCheck,
                CruiserCheck,
                SubCheck,
                DestroyerCheck
            };

            HorizontalRadio.Checked = true;

            //Initialize button grid
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PlanningButton b = new PlanningButton((short)i, (short)j);
                    b.Name = string.Format("Button{0}{1}", i, j);
                    b.Text = string.Format("{0}{1}", i, j);
                    b.Dock = DockStyle.Fill;
                    PlanningTable.Controls.Add(b, i, j);
                    b.Click += (sender, e) => ShipCoordButton_Click(sender, e, b.xCoord, b.yCoord);
                }
            }
        }
        
        private void ShipCoordButton_Click(object sender, EventArgs e, short x, short y)
        {
            int ind = x * 10 + y;
            var pg = appState.planningGrid;
            removeCurrentShip();
            currentShip.horizontal = HorizontalRadio.Checked;
            if (currentShip.horizontal)
            {
                if(x + currentShip.length <= 10)
                {
                    for (int i = 0; i < currentShip.length; i++)
                    {
                        if (pg[x + i][y] == 1) { ReadyCheck(); return; } //Invalid position: Ship Overlap
                    }
                    for (int i = 0; i < currentShip.length; i++)
                    {
                        pg[x + i][y] = 1;
                        PlanningTable.Controls[ind + (i * 10)].Enabled = false;
                    }
                    currentShip.position = new Tuple<short, short>(x, y);
                    ReadyCheck();
                }
                else
                {
                    //Invalid position: Out of bounds
                    currentShip.position = null;
                    ReadyCheck();
                    return;
                }
            }
            else
            {
                if(y + currentShip.length <= 10)
                {
                    for (int i = 0; i < currentShip.length; i++)
                    {
                        if (pg[x][y + i] == 1) { ReadyCheck(); return; } //Invalid position: Ship Overlap
                    }
                    for (int i = 0; i < currentShip.length; i++)
                    {
                        pg[x][y + i] = 1;
                        PlanningTable.Controls[ind + i].Enabled = false;
                    }
                    currentShip.position = new Tuple<short, short>(x, y);
                    ReadyCheck();
                }
                else
                {
                    //Invalid position: Out of bounds
                    currentShip.position = null;
                    ReadyCheck();
                    return;
                }
            }
        }

        public void removeCurrentShip()
        {
            if(currentShip.position == null) { return; }
            var pg = appState.planningGrid;
            int x = currentShip.position.Item1;
            int y = currentShip.position.Item2;
            int ind = x * 10 + y;
            if (currentShip.horizontal)
            {
                for (int i = 0; i < currentShip.length; i++)
                {
                    pg[x + i][y] = 0;
                    PlanningTable.Controls[ind + (i * 10)].Enabled = true;
                }
                currentShip.position = null;
            }
            else
            {
                for (int i = 0; i < currentShip.length; i++)
                {
                    pg[x][y + i] = 0;
                    PlanningTable.Controls[ind + i].Enabled = true;
                }
                currentShip.position = null;
            }
        }

        private void ReadyButton_Click(object sender, EventArgs e)
        {
            ReadyButton.Enabled = false;
            appState.CreateReadyConversation();
        }

        private void CarrierRadio_CheckedChanged(object sender, EventArgs e)
        {
            currentShip = shipList[0];
        }

        private void BattleshipRadio_CheckedChanged(object sender, EventArgs e)
        {
            currentShip = shipList[1];
        }

        private void CruiserRadio_CheckedChanged(object sender, EventArgs e)
        {
            currentShip = shipList[2];
        }

        private void SubRadio_CheckedChanged(object sender, EventArgs e)
        {
            currentShip = shipList[3];
        }

        private void DestroyerRadio_CheckedChanged(object sender, EventArgs e)
        {
            currentShip = shipList[4];
        }

        private void ReadyCheck()
        {
            bool ready = true;
            for (int i = 0; i < shipList.Count; i++)
            {
                if (shipList[i].position != null)
                {
                    shipCheckList[i].Visible = true;
                }
                else
                {
                    ready = false;
                    shipCheckList[i].Visible = false;
                }
            }
            if (ready)
            {
                ReadyButton.Enabled = true;
            }
            else
            {
                ReadyButton.Enabled = false;
            }
        }

        private void PlanningScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            appState.subSystem.stop();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }

        private void HorizontalRadio_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void VerticalRadio_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
