using System.Collections.Generic;

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
    }
}
