using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;

namespace Lobby.AppLogic
{
    public class LobbyPlayer
    {
        public IPEndPoint EP { get; set; }
        public int wins { get; set; } = 0;
        public int losses { get; set; } = 0; 
        public string name { get; set; }
        public short Id { get; set; }

        public LobbyPlayer(IPEndPoint ep, short id, string name)
        {
            this.EP = ep;
            this.Id = id;
            this.name = name;
        }

        public void addWin()
        {
            wins += 1;
        }

        public void addLoss()
        {
            losses += 1;
        }

    }
}
