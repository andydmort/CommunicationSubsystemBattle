using CommSubSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;

namespace Player
{
    public class PlayerAppState : AppState
    {
        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        private string privateKey;
        public string publicKey;
        public SubSystem subSystem;
        private WaitingRoom waitingRoom;
        private PlanningScreen planningScreen;
        private GameScreen gameScreen;

        public short gameId { get; set; }
        public List<List<short>> planningGrid;
        public Tuple<short, short> shotCoordinates;
        public IPEndPoint lobbyEndPoint;
        public IPEndPoint GMEndPoint;
        public string playerName;
        public short playerID;
        public string opponentName;
        public bool turn;
        public bool won = false;
        public bool end = false;
        public bool lastShotHit;
        public short lastX;
        public short lastY;
        public bool gameActive = false;
        public bool ended = false;

        public PlayerAppState(WaitingRoom waitingRoom)
        {
            privateKey = rsa.ToXmlString(true);
            publicKey  = rsa.ToXmlString(false);

            this.waitingRoom = waitingRoom;
            waitingRoom.appState = this;
            planningScreen = waitingRoom.planningScreen;
            planningScreen.appState = this;
            gameScreen = waitingRoom.gameScreen;
            gameScreen.appState = this;

            InitPlanningGrid();
        }

        public void Reset()
        {
            gameId = 0;
            planningGrid = null;
            shotCoordinates = null;
            lobbyEndPoint = null;
            subSystem.tcpcomm.closeConnection(GMEndPoint);
            GMEndPoint = null;
            playerName = null;
            playerID = -1;
            opponentName = null;
            turn = false;
            won = false;
            end = false;
            lastShotHit = false;
            lastX = -1;
            lastY = -1;
            gameActive = false;
            waitingRoom.resetText();
            planningScreen.InitializeStates();
            gameScreen.ResetColorButtons();
        }

        public byte[] decrypt(byte[] input)
        {
            rsa.FromXmlString(privateKey);
            byte[] decrypted = rsa.Decrypt(input, false);
            return decrypted;
        }

        private void InitPlanningGrid()
        {
            planningGrid = new List<List<short>>();
            for (int i = 0; i < 10; i++)
            {
                List<short> g = new List<short>();
                for(int j = 0; j < 10; j++)
                {
                    g.Add(0);
                }
                planningGrid.Add(g);
            }
        }

        public void SetTurn()
        {
            if (gameScreen.InvokeRequired)
            {
                gameScreen.Invoke(new Action(() => { gameScreen.SetTurn(); }));
            }
        }

        public void ColorButtonResult(bool turn, short x, short y, bool result)
        {
            if (gameScreen.InvokeRequired)
            {
                gameScreen.Invoke(new Action(() => {gameScreen.ColorButtonResult(turn, x, y, result); }));
            }
            
        }

        public void InformConnectionSuccess()
        {
            waitingRoom.EnterWaiting();
        }

        public void MoveToPlanning()
        {
            if (gameScreen.InvokeRequired)
            {
                gameScreen.Invoke(new Action(() => { gameScreen.Hide(); }));
            }
            if (waitingRoom.InvokeRequired)
            {
                waitingRoom.Invoke(new Action(() => { waitingRoom.Hide(); }));
            }
            if (planningScreen.InvokeRequired)
            {
                planningScreen.Invoke(new Action(() => { planningScreen.Show(); }));
            }
        }

        public void MoveToGame()
        {
            this.ended = false;   
            if (waitingRoom.InvokeRequired)
            {
                waitingRoom.Invoke(new Action(() => { waitingRoom.Hide(); }));
            }
            if (planningScreen.InvokeRequired)
            {
                planningScreen.Invoke(new Action(() => { planningScreen.Hide(); }));
            }
            if (gameScreen.InvokeRequired)
            {
                gameScreen.Invoke(new Action(() => { gameScreen.Show(); }));
                gameScreen.Invoke(new Action(() => { gameScreen.LoadPlayerGrid(); }));
            }
        }

        public void MoveToWaiting()
        {
            if (planningScreen.InvokeRequired)
            {
                planningScreen.Invoke(new Action(() => { planningScreen.Hide(); }));
            }
            if (gameScreen.InvokeRequired)
            {
                gameScreen.Invoke(new Action(() => { gameScreen.Hide(); }));
            }
            if (waitingRoom.InvokeRequired)
            {
                waitingRoom.Invoke(new Action(() => { waitingRoom.Show(); }));
            }
        }

        public void dangerousMoveToWaiting()
        {
            planningScreen.Hide();
            gameScreen.Hide();
            waitingRoom.Show();
        }
        
        public void RevealCoordinate()
        {
            gameScreen.ColorButtonResult(turn, lastX, lastY, lastShotHit);
        }

        public void CreateReadyConversation()
        {
            if(GMEndPoint == null) { return; }
            Conversation conversation = subSystem.conversationFactory.CreateFromConversationType<Board>();
            conversation.RemoteEndPoint = GMEndPoint;
            conversation.Launch();
        }

        public void CreatePlayerJoinConversation()
        {
            if (lobbyEndPoint == null) { return; }
            Conversation conversation = subSystem.conversationFactory.CreateFromConversationType<PlayerJoinLobby>();
            conversation.RemoteEndPoint = lobbyEndPoint;
            conversation.Launch();
        }

        public void CreateShotConversation()
        {
            if (GMEndPoint == null) { return; }
            Conversation conversation = subSystem.conversationFactory.CreateFromConversationType<Shot>();
            conversation.RemoteEndPoint = GMEndPoint;
            conversation.Launch();
        }
    }
}
