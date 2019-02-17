using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RacePrototype
    {
        private AffinityMap _affinities;
        private Stack<Tuple<string, int>> _wildcards;
        private Name _name;
        public Name RaceName { get { return _name; } }


        public RacePrototype(Name name)
        {
            _affinities = new AffinityMap();
            _wildcards = new Stack<Tuple<string, int>>();
            _name = name;
        }

        public Race Finalize()
        {
            AffinityMap finalAffinities = new AffinityMap(_affinities);
            foreach (var wildcard in _wildcards)
            {
                finalAffinities.ResolveWildcard(wildcard.Item1, wildcard.Item2);
            }

            return new Race(finalAffinities, _name);
        }

        public Race FinalizeAgainstMap(AffinityMap map)
        {
            AffinityMap finalAffinities = new AffinityMap(_affinities);
            foreach (var wildcard in _wildcards)
            {
                finalAffinities.ResolveWildcardAgainstMap(wildcard.Item1, wildcard.Item2, map);
            }

            return new Race(finalAffinities, _name);
        }

        public void SetAffinity(string aspect, int affinity)
        {
            _affinities.SetAffinity(aspect, affinity);
        }

        public void SetWildcard(string pool, int affinity)
        {
            _wildcards.Push(new Tuple<string, int>(pool, affinity));
        }
    }
}
