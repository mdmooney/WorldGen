using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RiverSegment
    {
        public Hex.Side? EntrySide { get; set; }
        public Hex.Side? ExitSide { get; set; }

        public RiverSegment PrevSegment { get; set; }
        public RiverSegment NextSegment { get; set; }

        public River ParentRiver { get; private set; }

        public RiverSegment(River river)
        {
            ParentRiver = river;
        }
    }
}
