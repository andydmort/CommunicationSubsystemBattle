using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using MessageTypes;

namespace ComSubLayer
{
    public class Envelope
    {
        public IPEndPoint To { get; set; }
        public IPEndPoint From { get; set; }
        public Message message;
        public bool isTcp;

        public Envelope(Message message, IPEndPoint to, IPEndPoint from, bool tcp)
        {
            this.message = message;
            this.To = to;
            this.From = from;
            this.isTcp = tcp;
        }

    }
}
