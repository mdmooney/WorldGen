namespace WorldGen
{
    /**
     * <summary>
     * HexExpander for altering ElevationLevels of Hexes on a HexMap.
     * Conceptually, this is meant to progressively raise the elevation
     * of a landmass randomly in several passes. This class does not lower
     * the ElevationLevel of any hex at any point.
     * 
     * See also <seealso cref="LayeredExpander"/>.
     * </summary>
     */
    class HeightExpander : LayeredExpander
    {
        /**
         * <summary>
         * Constructor for HeightExpander takes the map to alter.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         */
        public HeightExpander(HexMap map) : base(map) { }

        /**
         * <summary>
         * Hex modification method for HeightExpander attempts to raise the ElevationLevel of the Hex
         * at given Coords by one step. See <see cref="HexMap.Raise(Coords)"/>.
         * </summary>
         * <param name="coords">Coords of the Hex to raise.</param>
         */
        protected override bool LayeredModHex(Coords coords)
        {
            return _map.Raise(coords);
        }
    }
}
