namespace WorldGen
{
    /**
     * <summary>
     * HexExpander for producing landmasses on a HexMap.
     * 
     * See also <seealso cref="HexExpander"/>.
     * </summary>
     */
    class LandmassExpander : HexExpander
    {

        /**
         * <summary>
         * Sole constructor just takes a map on which to generate landmasses.
         * </summary>
         */
        public LandmassExpander(IRandomGen rand, HexMap map) : base(rand, map)
        {}

        /**
         * <summary>
         * Expansion validity check. Checks if land placement is valid
         * at the given Coords. See <see cref="HexMap.CanPlaceAt(Coords)"/>.
         * </summary>
         * <param name="coords">Coords to query.</param>
         * <returns>
         * True if land can be placed at the given Coords, false otherwise.
         * </returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            return _map.CanPlaceAt(coords);
        }

        /**
         * <summary>
         * Hex modification. Simply places land at the Hex at given Coords.
         * </summary>
         * <param name="coords">Coords of Hex to update.</param>
         * <returns>
         * True if land was successfuly placed at <paramref name="coords"/>.
         * False otherwise.
         * </returns>
         */
        protected override bool ModHex(Coords coords)
        {
            return _map.PlaceLand(coords);
        }

        /**
         * <summary>
         * Update method for Hexes adjacent to Coords that were altered in
         * this round of expansion, but not expanded from themselves.
         * Sets all Hexes adjacent to Hexes updated in this expansion to be
         * unplaceable. The intent of this is to ensure that new landmasses
         * produced in later iterations will not collide with this one, leaving
         * at least one hex between them.
         * </summary>
         */
        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            _map.SetPlaceable(coords, false);
        }
    }
}
