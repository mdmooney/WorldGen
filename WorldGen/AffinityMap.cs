using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class AffinityMap
    {
        // Bounds for max and min aspect affinities
        private static readonly int MaxAffinity = 5;
        private static readonly int MinAffinity = -5;

        private Dictionary<string, int> _affinities;
        private AspectGlossary _aspectGlossary = AspectGlossary.GetInstance();

        public List<string> AspectList
        {
            get
            {
                return _affinities.Keys.ToList();
            }
        }

        public int Count
        {
            get
            {
                return _affinities.Count;
            }
        }

        public AffinityMap()
        {
            _affinities = new Dictionary<string, int>();
        }

        public AffinityMap(AffinityMap other)
        {
            _affinities = new Dictionary<string,int>(other._affinities);
        }

        public int GetAffinity(string aspect)
        {
            if (_affinities.ContainsKey(aspect))
                return _affinities[aspect];

            // If an aspect is not represented, we consider it to have an affinity
            // of 0, i.e. no positive or negative relation.
            return 0;
        }

        public void SetAffinity(string aspect, int val)
        {
            // validate the aspect
            if (!_aspectGlossary.Contains(aspect))
            {
                throw InvalidAspectException.FromAspect(aspect);
            }

            // bound the affinity value
            int boundedVal = val;
            if (val > MaxAffinity) boundedVal = MaxAffinity;
            else if (val < MinAffinity) boundedVal = MinAffinity;

            // TODO: confirm that this is a real aspect, as listed in the glossary
            // TODO: handle opposed and related aspects, etc.

            _affinities[aspect] = boundedVal;
        }

        public void MaximizeAffinity(string aspect)
        {
            _affinities[aspect] = MaxAffinity;
        }

        public void MinimizeAffinity(string aspect)
        {
            _affinities[aspect] = MinAffinity;
        }


        public List<string> IntersectAspects(List<string> aspects)
        {
            var myAspects = _affinities.Keys;
            List<string> rv = aspects.Intersect(myAspects).ToList();
            return rv;
        }

        public AffinityMap FilterByPool(string pool)
        {
            if (!_aspectGlossary.HasPool(pool))
            {
                return new AffinityMap(this);
            }

            var poolMembers = _aspectGlossary.GetPool(pool);
            var theseAspects = _affinities.Keys;
            var commonMembers = theseAspects.Intersect(poolMembers);

            AffinityMap newMap = new AffinityMap();
            foreach (var member in commonMembers)
            {
                newMap.SetAffinity(member, _affinities[member]);
            }

            return newMap;
        }
    }
}
