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

        protected string Combined { get { return Plural + Single + Adjective; } }

        public Name(string plural, string single, string adjective)
        {
            Plural = plural;
            Single = single;
            Adjective = adjective;
        }

        public override bool Equals(object obj)
        {
            Name other = obj as Name;
            if (other == null)
                return false;
            return Combined.Equals(other.Combined);
        }

        // hash code is based on hash codes of all combined names
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Plural.GetHashCode();
            hash = hash * 31 + Single.GetHashCode();
            hash = hash * 31 + Adjective.GetHashCode();
            return base.GetHashCode();
        }
    }
}
