using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Lobby.AppLogic
{
    public class LobbyGM
    {
        public IPEndPoint TCPEP { get; set; }
        public IPEndPoint UDPEP { get; set; }
        public short gameID { get; set; }

        public LobbyGM(IPEndPoint TcpEp,IPEndPoint UdpEp, short gameid)
        {
            this.TCPEP = TcpEp;
            this.UDPEP = UdpEp;
            this.gameID = gameid;
        }
    }
}
