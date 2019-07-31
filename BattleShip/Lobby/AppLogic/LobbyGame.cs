using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lobby.AppLogic
{
    public class LobbyGame
    {
        public LobbyGM GM;
        public LobbyPlayer Pl1;
        public LobbyPlayer Pl2;
        public short gameId; // AKA game manager ID. 

        public LobbyGame(LobbyGM gm, LobbyPlayer pl1, LobbyPlayer pl2)
        {
            this.GM = gm;
            this.Pl1 = pl1;
            this.Pl2 = pl2;
            this.gameId = gm.gameID;
        }
    }
}
