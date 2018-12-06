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
        private Hex.ElevationLevel _elevation;

        /**
         * <summary>
         * Constructor for HeightExpander takes the map on which to alter
         * ElevationLevels, as well as a "pass" value, which indicates the
         * highest level the HeightExpander will raise a given Hex to.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         * <param name="pass">The maximum height value for this round of expansion.</param>
         */
        public HeightExpander(HexMap map, int pass) : base(map)
        {
            _elevation = (Hex.ElevationLevel)pass;
        }


        /**
         * <summary>
         * Expansion validity check. Simply checks to ensure that the Hex at
         * the given Coords is below the max height level (and is thus eligible
         * for further elevation).
         * </summary>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * True if the elevation of the Hex at <paramref name="coords"/> is
         * below the maximum value for this HeightExpander, false otherwise.
         * </returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            return _map.GetElevationAt(coords) < _elevation;
        }

        /**
         * <summary>
         * Update method for Hexes adjacent to Coords that were altered in
         * this round of expansion, but not expanded from themselves.
         * Does not do anything for this class at present.
         * </summary>
         */
        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // no implementation at this point
            // may add smoothing operation here of some kind
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
            return (_map.Raise(coords));
        }
    }
}
