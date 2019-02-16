using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class Name
    {
        public string Plural { get; set; }
        public string Single { get; set; }
        public string Adjective { get; set; }

        public Name(string plural, string single, string adjective)
        {
            Plural = plural;
            Single = single;
            Adjective = adjective;
        }
    }
}
