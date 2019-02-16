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
    }
}
