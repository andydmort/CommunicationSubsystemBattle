using System;
using System.Net.Sockets;
using ComSubLayer;

namespace Communicators
{
    public abstract class Communicator
    {
        public abstract void send(Envelope env);
        //public abstract Envelope recieve();
    }
}
