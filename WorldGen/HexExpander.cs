using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{

    /* <summary>
     * Abstract parent class for hex expanders, i.e. classes which expand a
     * region in a HexMap from a single Hex, according to some set of rules.
     * 
     * The expansion is semi-random and is intended to create an organic
     * spread.
     * </summary>
     */
    abstract class HexExpander
    {
        protected HexMap _map;

        protected int _totalHexes;
        protected int _remainingHexes;
        protected List<Coords> _validHexes;

        protected abstract bool CanExpandTo(Coords coords);
        protected abstract bool ModHex(Coords coords);
        protected abstract void FinishAdjacentUnexpanded(Coords coords);

        public HexExpander(HexMap map)
        {
            _map = map;
        }

        public List<Coords> Expand(List<Coords> validHexes, int size)
        {
            int width = _map.width;
            int height = _map.height;
            _totalHexes = _remainingHexes = size;
            _validHexes = validHexes;

            Random rnd = new Random();

            List<Coords> unexpanded = new List<Coords>();
            List<Coords> expandAgain = new List<Coords>();
            List<Coords> tempInvalids = new List<Coords>();

            List<Coords> rv = new List<Coords>();

            // Select a valid hex at random to start from
            bool placedSeed = false;
            int attempts = 0;
            int totalValidHexes = validHexes.Count;
            while (!placedSeed 
                    && (attempts < totalValidHexes))
            {
                int i = rnd.Next(totalValidHexes);
                Coords seedCoords = validHexes[i];
                placedSeed = ModHex(seedCoords);
                if (placedSeed)
                {
                    _remainingHexes--;
                    unexpanded.Add(seedCoords);
                }
                else attempts++;
            }

            while (_remainingHexes > 0)
            {
                if (unexpanded.Count == 0)
                {
                    if (expandAgain.Count() > 0)
                    {
                        unexpanded = expandAgain;
                    }
                    else
                    {
                        break;
                    }
                }
                Coords startPoint = unexpanded[rnd.Next(unexpanded.Count)];

                Dictionary<Hex.Side, Coords> adj = GetFilteredAdjacency(startPoint);

                int roll = rnd.Next(adj.Count + 1) + RollModifier();
                List<Hex.Side> contigs = FindNContiguousValidSides(startPoint, roll);

                // todo: reduce expected number of contigs and retry if we can't find enough contiguous hexes
                if (contigs.Count > 0)
                {
                    Hex.Side placeSide = contigs[rnd.Next(contigs.Count)];
                    int count = 0;
                    while (count < roll)
                    {
                        if (adj.ContainsKey(placeSide))
                        {
                            Coords placeLoc = adj[placeSide];
                            bool placed = ModHex(placeLoc);
                            if (placed)
                            {
                                rv.Add(placeLoc);
                                _remainingHexes--;
                                unexpanded.Add(placeLoc);
                            }
                        }
                        placeSide = Hex.RotateSideClockwise(placeSide);
                        count++;
                    }

                    while (count < 6)
                    {
                        if (adj.ContainsKey(placeSide))
                        {
                            Coords invalidatedSide = adj[placeSide];
                        }
                        placeSide = Hex.RotateSideClockwise(placeSide);
                        count++;
                    }
                }

                unexpanded.Remove(startPoint);
                if (roll < adj.Count)
                    expandAgain.Add(startPoint);
            }

            foreach (Coords remainder in unexpanded)
            {
                Dictionary<Hex.Side, Coords> toGo = GetFilteredAdjacency(remainder);
                foreach (Coords toGoInstance in toGo.Values)
                {
                    FinishAdjacentUnexpanded(remainder);
                }
            }

            return rv;
        }

        private List<Hex.Side> FindNContiguousValidSides(Coords coords, int n)
        {
            // Can't have more than six sides
            if (n > 6) n = 6;
            List<Hex.Side> goodSides = new List<Hex.Side>();

            Dictionary<Hex.Side, Coords> adj = GetFilteredAdjacency(coords);

            foreach (Hex.Side side in adj.Keys)
            {
                int count = 1;
                Hex.Side checkSide = Hex.RotateSideClockwise(side);
                while (adj.ContainsKey(side) && count < 6)
                {
                    count++;
                    checkSide = Hex.RotateSideClockwise(side);
                }
                if (count >= n)
                {
                    goodSides.Add(side);
                }
            }

            return goodSides;
        }

        private Dictionary<Hex.Side, Coords> GetFilteredAdjacency(Coords coords)
        {

            Dictionary<Hex.Side, Coords> allAdjacent = _map.GetAllAdjacentCoords(coords);

            Dictionary<Hex.Side, Coords> rv 
                = allAdjacent.Where(x => (_validHexes.Contains(x.Value) && CanExpandTo(x.Value)))
                                                         .ToDictionary(x => x.Key, x => x.Value);

            return rv;
        }

        protected virtual int RollModifier()
        {
            double ratio = _remainingHexes / _totalHexes;
            Random rnd = new Random();
            int mod = 0;

            while ((mod < 6)
                    && (ratio > 0))
            {
                double comp = rnd.NextDouble();
                if (comp <= ratio)
                {
                    mod++;
                    ratio /= 2;
                }
                else break;
            }
            return mod;
        }

    }
}
