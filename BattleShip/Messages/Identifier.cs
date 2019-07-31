using System;
using System.Collections.Generic;
using System.IO;

namespace Messages
{
    public class Identifier
    {
        public short Pid { get; private set; } = 0;
        public short Seq { get; private set; } = 0;

        public override string ToString()
        {
            return Pid.ToString() + "." + Seq.ToString();
        }

        public Identifier(short pid, short seq)
        {
            Pid = pid;
            Seq = seq;
        }
        public short getPid() { return this.Pid; }
        public short getSeq() { return this.Seq; }

        public bool Equals(Identifier obj)
        {

            if(this.Pid == obj.Pid && this.Seq == obj.Seq)
            {
                return true;
            }

            return false;

        }
    }
}
