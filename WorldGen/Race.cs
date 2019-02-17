using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class Race
    {
        private AffinityMap _affinities;
        private Name _name;
        public Name RaceName { get { return _name; } }

        public AffinityMap Affinities
        {
            get { return new AffinityMap(_affinities); }
        }

        public Race(AffinityMap affinities, Name name)
        {
            _affinities = affinities;
            _name = name;
        }

        public override string ToString()
        {
            return _name.Plural;
        }

        // equality comparator based solely on name, for now
        public override bool Equals(object obj)
        {
            Race other = obj as Race;
            if (other == null)
                return false;
            return RaceName.Equals(other.RaceName);
        }

        // hash code is based solely on name for now
        public override int GetHashCode()
        {
            return RaceName.GetHashCode();
        }
    }
}
