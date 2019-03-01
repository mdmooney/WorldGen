using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("WorldGenTests")]
namespace WorldGen
{
    class AffinityMap
    {
        // Bounds for max and min aspect affinities
        public static readonly int MaxAffinity = 5;
        public static readonly int MinAffinity = -MaxAffinity;

        private Dictionary<string, int> _affinities;
        private AspectGlossary _aspectGlossary = AspectGlossary.GetInstance();

        private static Random _rand = new Random();

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
            // validate the aspect
            if (!_aspectGlossary.Contains(aspect))
            {
                throw InvalidAspectException.FromAspect(aspect);
            }

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

            // TODO: handle opposed and related aspects, etc.

            _affinities[aspect] = boundedVal;
        }

        public void MaximizeAffinity(string aspect)
        {
            SetAffinity(aspect, MaxAffinity);
        }

        public void MinimizeAffinity(string aspect)
        {
            SetAffinity(aspect, MinAffinity);
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

        public string SelectAspectByAffinity()
        {
            var randomTable = new RandomTable<string>();

            foreach (var kvp in _affinities.ToList())
            {
                randomTable.Add(kvp.Key, kvp.Value);
            }

            return randomTable.Roll();
        }

        public void ResolveWildcardAgainstMap(string pool, int affinity, AffinityMap other)
        {
            AffinityMap filtered = other.FilterByPool(pool);
            if (filtered.Count == 0)
                ResolveWildcard(pool, affinity);
            else
            {
                string aspect = filtered.SelectAspectByAffinity();
                SetAffinity(aspect, affinity);
            }
        }

        public void ResolveWildcard(string pool, int affinity)
        {
            var poolList = _aspectGlossary.GetPool(pool).ToList();
            int i = _rand.Next(poolList.Count);
            string aspect = poolList[i];

            // make sure we're not overwriting an existing affinity
            while (GetAffinity(aspect) != 0)
            {
                i = _rand.Next(poolList.Count);
               aspect = poolList[i];
            }

            SetAffinity(poolList[i], affinity);
        }

        public static int CombineAffinities(int val1, int val2)
        {
            return (val1 * val2) / MaxAffinity;
        }

        public AffinityMap CombineWith(AffinityMap other)
        {
            AffinityMap combined = new AffinityMap();
            foreach (var aspect in _affinities.Keys)
            {
                int otherAffinity = other.GetAffinity(aspect);
                if (otherAffinity != 0)
                {
                    int combinedVal = CombineAffinities(GetAffinity(aspect), other.GetAffinity(aspect));
                    if (combinedVal != 0)
                    {
                        combined.SetAffinity(aspect, combinedVal);
                    }
                }
            }

            return combined;
        }

        public int GetSimilarityTo(AffinityMap other)
        {
            int total = 0;
            foreach (var aspect in _affinities.Keys)
            {
                int otherAffinity = other.GetAffinity(aspect);
                if (otherAffinity != 0)
                {
                    total += (GetAffinity(aspect) * other.GetAffinity(aspect)) / 2;
                }
            }

            return total;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var affinity in _affinities)
            {
                sb.Append(affinity.Key);
                sb.Append(" : ");
                sb.Append(affinity.Value);
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
