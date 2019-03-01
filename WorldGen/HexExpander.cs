using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldGen
{

    /** <summary>
     * Abstract parent class for hex expanders, i.e. classes which expand a
     * region in a HexMap starting from a single Hex, and then expanding further
     * from modified hexes. This may happen multiple times, according to a set
     * of rules defined by subclasses.
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

        private static RandomGen _rand = new RandomGen();

        private bool _finalizeEarly = false;

        /// <summary>
        /// Whether this HexExpander allows re-expansion from Hexes that have
        /// already been expanded from previously.
        /// </summary>
        public virtual bool AllowsReexpansion { get { return true; } }

        // ------------ Abstract methods ------------
        // These methods must be overridden to dictate the rules of expansion.

        /** <summary>
         * Determines the criteria that constitutes a valid or invalid destination
         * or expansion.
         * </summary>
         * <param name="coords">Coords for which to check validity of expansion.</param>
         * <returns>True if coords are valid to expand to, false otherwise.</returns>
         */
        protected abstract bool CanExpandTo(Coords coords);

        /** <summary>
         Attempt to modify the given hex in accordance with whatever this
         expansion is trying to do (e.g. change the elevation).
         * </summary>
         * <param name="coords">Coords for which the corresponding hex will be possibly modified.</param>
         * <returns>True if the hex at coords was successfully modified, false otherwise.</returns>
         */
        protected abstract bool ModHex(Coords coords);

        /** <summary>
         * Method called on all hexes adjacent to hexes that were not expanded from
         * (i.e. hexes that could have been expanded from, but we hit some end condition,
         * like reaching the total number of desired hexes, before that happened).
         * </summary>
         * <param name="coords">Coords of a hex adjacent to some unexpanded hex.</param>
         */
        protected abstract void FinishAdjacentUnexpanded(Coords coords);


        // ------------ Concrete methods ------------

        /**
         * <summary>
         * Sole constructor of a HexExpander just takes the HexMap which will be
         * altered by the hex expansion.
         * </summary>
         */
        protected HexExpander(HexMap map)
        {
            _map = map;
        }

        /** <summary>
         * Expand the given property in the map, within a set of coords predetermined
         * to point to hexes valid for expansion.
         * </summary>
         * <param name="validHexes">List of all coords containing hexes which are valid to expand to.</param>
         * <param name="size">The number of hexes to expand.</param>
         * <return>The list of all coords whose corresponding hexes were modified by this expansion.</return>
         */
        public List<Coords> Expand(List<Coords> validHexes, int size)
        {
            int width = _map.Width;
            int height = _map.Height;
            _totalHexes = _remainingHexes = size;
            _validHexes = validHexes;

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
                int i = _rand.GenerateInt(totalValidHexes);
                Coords seedCoords = validHexes[i];
                if (CanExpandFirst(seedCoords))
                {
                    placedSeed = ModHex(seedCoords);
                    if (placedSeed)
                    {
                        _remainingHexes--;
                        rv.Add(seedCoords);
                        unexpanded.Add(seedCoords);
                        continue;
                    }
                }
                attempts++;
            }

            while ((_remainingHexes > 0)
                    && !_finalizeEarly)
            {
                if (unexpanded.Count == 0)
                {
                    if (AllowsReexpansion
                        && (expandAgain.Count() > 0))
                    {
                        unexpanded = expandAgain;
                    }
                    else
                    {
                        break;
                    }
                }

                // Pick a hex to expand at random from the unexpanded list
                Coords startPoint = unexpanded[_rand.GenerateInt(unexpanded.Count)];
                Dictionary<Hex.Side, Coords> adj = GetFilteredAdjacency(startPoint);

                // Determine at random how many hexes will be affected in this expansion
                // (i.e. how many adjacent hexes we'll attempt to modify in this round)
                int roll = RollBaseExpansion(adj.Count + 1) + RollModifier();
                List<Hex.Side> contigs = FindNContiguousValidSides(startPoint, roll);

                if (contigs.Count > 0)
                {
                    Hex.Side placeSide = contigs[_rand.GenerateInt(contigs.Count)];
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

        /** <summary>
         * Determines if a first modification in an expansion is valid.
         * Similar to CanExpandTo(), except that method is for expansions after the first,
         * and this is for the initial placement.
         * </summary>
         * <param name="coords">Coords to check for validity.</param>
         * <returns>True if <paramref name="coords"/> are okay for first expansion, false otherwise.</returns>
         */
        protected virtual bool CanExpandFirst(Coords coords)
        {
            return true;
        }

        /** <summary>
         * Given some coordinates, find all sides of the given tile such that the neighbour
         * on that side, and all subsequent neighbours in a contiguous clockwise rotation
         * for n turns, are valid for placement.
         * </summary>
         * <remarks>
         * If more sides are requested than the maximum number of adjacent tiles allows
         * (6, for a hexagonal grid), the maximum number of sides will be used as a ceiling.
         * </remarks>
         * <param name="coords">The coordinates (start point) to search from.</param>
         * <param name="n">The number of contiguous sides to find.</param>
         * <return>A list of all sides which have <paramref name="n"/> clockwise contiguous valid neighbours.</return>
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

        /** <summary>
         * Get a list of all coords adjacent to the given coords, filtered such that:
         *  - All returned coords are contained in the valid hex list
         *  - All returned coords can be expanded to, per implementation of CanExpandTo()
         * </summary>
         * <param name="coords">Coords for which to find valid adjacent hexes.</param>
         * <returns>A dictionary of valid coordinates adjacent to <paramref name="coords"/>, keyed by which
         * side of the hex at the given coords they are adjacent to.</returns>
         */
        private Dictionary<Hex.Side, Coords> GetFilteredAdjacency(Coords coords)
        {
            return _map.GetFilteredAdjacency(coords, (x => _validHexes.Contains(x) && CanExpandTo(x)));
        }

        /** <summary>
         * Generates a modifier to add onto the base roll, used to determine the number of sides
         * to expand out to from a starting hex.
         * </summary>
         * <remarks>
         * This modifier is random, but will likely be higher when there are more hexes that
         * have not yet been expanded to. This is done to favour a more blob-shaped core with
         * branches/peninsulae as generation goes on.
         * </remarks>
         * <returns>A modifier not exceeding Hex.SIDES, likely higher when there are more hexes
         * that have not yet been placed.</returns>
         */
        protected virtual int RollModifier()
        {
            double ratio = _remainingHexes / _totalHexes;
            int mod = 0;

            while ((mod < Hex.SIDES)
                    && (ratio > 0))
            {
                double comp = _rand.GenerateDouble();
                if (comp <= ratio)
                {
                    mod++;
                    ratio /= 2;
                }
                else break;
            }
            return mod;
        }

        /** <summary>
         * Randomly determine how many sides to expand out into from a starting hex.
         * </summary>
         * <param name="adjCount">The maximum number of sides to expand into.</param>
         * <returns>A random integer in range [0, adjCount].</returns>
         */
        protected virtual int RollBaseExpansion(int adjCount)
        {
            return _rand.GenerateInt(adjCount);
        }

        /** <summary>
         * Simple switch method to end expansion early. This will enable a flag that,
         * when set, will prevent any further iterations of expansion from happening.
         * </summary>
         * <remarks>
         * Use of FinalizeEarly may be desirable when a subclass of this class has a
         * specific end condition that would otherwise be ignored by expansion.
         * </remarks>
         */
        protected void FinalizeEarly()
        {
            _finalizeEarly = true;
        }

    }
}
