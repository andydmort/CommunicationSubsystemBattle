using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class PlanningShip
    {
        public short length { get; }
        public Tuple<short, short> position { get; set; }
        public bool horizontal { get; set; }

        public PlanningShip(short length)
        {
            this.length = length;
            horizontal = true;
        }
    }
}
