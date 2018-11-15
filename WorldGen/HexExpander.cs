using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldGen
{

    /* <summary>
     * Abstract parent class for hex expanders, i.e. classes which expand a
     * region in a HexMap from a single Hex, possibly multiple times, according 
     * to some set of rules.
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


        // ------------ Abstract methods ------------
        // These methods must be overridden to dictate the rules of expansion.

        /* <summary>
         * Determines the criteria that constitutes a valid or invalid destination
         * for expansion.
         * </summary>
         * <returns>
         * True if coords are valid to expand to, false otherwise.
         * </returns>
         */
        protected abstract bool CanExpandTo(Coords coords);

        /* <summary>
         * Attempt to modify the given hex in accordance with whatever this
         * expansion is trying to do (e.g. change the elevation).
         * </summary>
         * <returns>
         * True if the hex at coords was successfully modified, false otherwise.
         * </returns>
         */
        protected abstract bool ModHex(Coords coords);

        /* <summary>
         * Method called on all hexes adjacent to hexes that were not expanded from
         * (i.e. hexes that could have been expanded from, but we hit some end condition,
         * like reaching the total number of desired hexes, before that happened).
         * </summary>
         */
        protected abstract void FinishAdjacentUnexpanded(Coords coords);


        // ------------ Concrete methods ------------

        public HexExpander(HexMap map)
        {
            _map = map;
        }

        /* <summary>
         * Expand the given property in the map, within a set of coords predetermined
         * to point to hexes valid for expansion.
         * </summary>
         * <param name="validHexes">List of all coords containing hexes which are valid to expand to.</param>
         * <param name="size">The number of hexes to expand.</param>
         * <return>The list of all coords whose corresponding hexes were modified by this expansion.</return>
         */
        public List<Coords> Expand(List<Coords> validHexes, int size)
        {
            int width = _map.width;
            int height = _map.height;
            _totalHexes = _remainingHexes = size;
            _validHexes = validHexes;

            Random rnd = new Random();

            // list of coords to be expanded out from, but which have not been expanded from yet
            List<Coords> unexpanded = new List<Coords>();
            // list of coords to expand from again
            List<Coords> expandAgain = new List<Coords>();

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

                // Pick a hex to expand from
                Coords startPoint = unexpanded[rnd.Next(unexpanded.Count)];
                Dictionary<Hex.Side, Coords> adj = GetFilteredAdjacency(startPoint);

                // Determine at random how many hexes will be affected in this expansion
                // (i.e. how many adjacent hexes we'll attempt to modify in this round)
                int roll = RollBaseExpansion(adj.Count + 1) + RollModifier();
                List<Hex.Side> contigs = FindNContiguousValidSides(startPoint, roll);

                // todo: reduce expected number of contigs and retry if we can't find enough contiguous hexes
                if (contigs.Count > 0)
                {
                    Hex.Side placeSide = contigs[rnd.Next(contigs.Count)];
                    int count = 0;

                    // Try to modify as many contiguous hexes as we randomly determined we would
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
                }

                // We've now expanded from these coords; remove them from the unexpanded list
                unexpanded.Remove(startPoint);
                // But if there are more adjacent hexes to expand to than we expanded into this round,
                // we can add this hex into the expand-again list
                if (roll < adj.Count)
                    expandAgain.Add(startPoint);
            }

            // Go through all remaining Coords that were not expanded from, and do something with
            // their adjacent hexes, depending on the implementation of FinishAdjacentUnexpanded().
            foreach (Coords remainder in unexpanded)
            {
                Dictionary<Hex.Side, Coords> toGo = GetFilteredAdjacency(remainder);
                foreach (Coords toGoInstance in toGo.Values)
                {
                    FinishAdjacentUnexpanded(toGoInstance);
                }
            }

            return rv;
        }

        /* <summary>
         * Given some coordinates, find all sides of the given tile such that the neighbour
         * on that side, and all subsequent neighbours in a contiguous clockwise rotation
         * for n turns, are valid for placement.
         * 
         * If more sides are requested than the maximum number of adjacent tiles allows
         * (6, for a hexagonal grid), the maximum number of sides will be used as a ceiling.
         * </summary>
         * <param name="coords">The coordinates (start point) to search from.</param>
         * <param name="n">The number of contiguous sides to find.</param>
         * <return>A list of all sides which have n clockwise contiguous valid neighbours.</return>
         */
        private List<Hex.Side> FindNContiguousValidSides(Coords coords, int n)
        {
            // Can't have more than the max number of sides
            if (n > Hex.SIDES) n = Hex.SIDES;
            List<Hex.Side> goodSides = new List<Hex.Side>();

            // Filter out any invalid sides
            Dictionary<Hex.Side, Coords> adj = GetFilteredAdjacency(coords);

            foreach (Hex.Side side in adj.Keys)
            {
                int count = 1;
                Hex.Side checkSide = Hex.RotateSideClockwise(side);
                while (adj.ContainsKey(side) && count < Hex.SIDES)
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

        /* <summary>
         * Get a list of all coords adjacent to the given coords, filtered such that:
         *  - All returned coords are contained in the valid hex list
         *  - All returned coords can be expanded to, per implementation of CanExpandTo()
         * </summary>
         */
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

            while ((mod < Hex.SIDES)
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

        protected virtual int RollBaseExpansion(int adjCount)
        {
            Random rnd = new Random();
            return rnd.Next(adjCount);
        }

    }
}
