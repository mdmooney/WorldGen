using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class River
    {
        public string Name { get; private set; }

        public RiverSegment FirstSeg { get; set; }
        public RiverSegment LastSeg { get; set; }

        public River()
        {
            Name = "River";
        }

        public River(string name)
        {
            Name = name;
        }
    }
}
