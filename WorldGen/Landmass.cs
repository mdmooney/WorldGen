using System.Collections.Generic;
using System;

namespace WorldGen
{
    class Landmass
    {
        /// <summary>
        /// This class represents a landmass, used mainly by the
        /// LandmassExpander. It tracks how many land Hexes have been placed, how
        /// many Hexes we're trying to place, and the Coords of modified Hexes.
        /// </summary>
        public int TotalHexes;
        public int RemainingHexes;
        public List<Coords> Hexes = new List<Coords>();
        public List<Coords> ShoreHexes = new List<Coords>();
        private AffinityMap _affinities;
        private IRandomGen _rand;
        public int Count { get { return Hexes.Count; } }

        public Landmass(IRandomGen rand)
        {
            _rand = rand;
        }

        public AffinityMap Affinities
        {
            get
            {
                if (_affinities != null)
                    return new AffinityMap(_affinities);
                return null;
            }
            set
            {
                _affinities = value;
            }
        }

        public Coords RandomCoords()
        {
            return new Coords(Hexes[_rand.GenerateInt(Hexes.Count)]);
        }

        public System.Collections.IEnumerable CoordsFromRandomPoint()
        {
            int r = _rand.GenerateInt(Hexes.Count);
            int i = r + 1;

            while (i != r)
            {
                if (i >= Hexes.Count)
                    i = 0;

                yield return new Coords(Hexes[i]);
                i++;
            }
        }
    }
}
