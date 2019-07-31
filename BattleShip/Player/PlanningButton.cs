using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public class PlanningButton : Button
    {
        public short xCoord { get; }
        public short yCoord { get; }

        public PlanningButton(short x, short y)
        {
            xCoord = x;
            yCoord = y;
        }
    }
}
