using System.Net;
using Messages;

namespace CommSubSys
{
    public class Envelope
    {
        public IPEndPoint To { get; set; }
        public IPEndPoint From { get; set; }
        public Message Message { get; set; }
        public bool isTcp { get; set; }

        public Envelope(Message message, IPEndPoint from, IPEndPoint to, bool tcp)
        {
            Message = message;
            To = to;
            From = from;
            isTcp = tcp;
        }

    }
}
