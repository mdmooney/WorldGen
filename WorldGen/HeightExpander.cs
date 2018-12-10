using System.Collections.Generic;

namespace WorldGen
{
    /**
     * <summary>
     * HexExpander for altering ElevationLevels of Hexes on a HexMap.
     * Conceptually, this is meant to progressively raise the elevation
     * of a landmass randomly in several passes. This class does not lower
     * the ElevationLevel of any hex at any point.
     * 
     * See also <seealso cref="HexExpander"/>.
     * </summary>
     */
    class HeightExpander : HexExpander
    {
        private List<Coords> _alreadyRaised;

        /**
         * <summary>
         * Constructor for HeightExpander takes the map on which to alter
         * ElevationLevels.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         */
        public HeightExpander(HexMap map) : base(map)
        {
            _alreadyRaised = new List<Coords>();
        }


        /**
         * <summary>
         * Expansion validity check. Simply checks to ensure that the Hex at
         * the given Coords has not already been raised in this pass.
         * </summary>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * True if the elevation of the Hex at <paramref name="coords"/> has
         * not yet been raised in this pass, false otherwise.
         * </returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            return !(_alreadyRaised.Contains(coords));
        }

        /**
         * <summary>
         * Update method for Hexes adjacent to Coords that were altered in
         * this round of expansion, but not expanded from themselves.
         * </summary>
         * <remarks>
         * For this class, we just clear the already-raised hex tracking
         * for the next round of expansion.
         * </remarks>
         */
        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // Just clear out already raised hexes for the next round
            // of expansion
            _alreadyRaised.Clear();
        }

        /**
         * <summary>
         * Hex modification. Attempts to raise the ElevationLevel of the Hex
         * at given Coords by one step. See <see cref="HexMap.Raise(Coords)"/>.
         * </summary>
         * <returns>True if the ElevationLevel was increased, false otherwise.</returns>
         */
        protected override bool ModHex(Coords coords)
        {
            if (_map.Raise(coords))
            {
                _alreadyRaised.Add(coords);
                return true;
            }
            return false;
        }
    }
}
