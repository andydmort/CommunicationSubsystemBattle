using System;
using System.Net.Sockets;
using System.Security.Cryptography;  

namespace CommSubSys
{
    public abstract class Communicator
    {
        public abstract void send(Envelope env);
        protected byte[] key = null;
        protected byte[] IV = null;

        public void setKeyAndIV(byte[] key, byte[] IV)
        {
            this.key = key;
            this.IV = IV;
        }

        //public abstract Envelope recieve();
    }
}
