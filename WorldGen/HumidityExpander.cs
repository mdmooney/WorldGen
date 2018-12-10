using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class HumidityExpander : HexExpander
    {
        private List<Coords> _alreadyLowered;

        /**
         * <summary>
         * Constructor for HumidityExpander takes the map on which to alter
         * HumidityLevels.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         */
        public HumidityExpander(HexMap map) : base(map)
        {
            _alreadyLowered = new List<Coords>();
        }


        /**
         * <summary>
         * Expansion validity check. Simply checks to ensure that the Hex at
         * the given Coords has not already been made more arid in this pass.
         * </summary>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * True if the humidity of the Hex at <paramref name="coords"/> has
         * not yet been lowered in this pass, false otherwise.
         * </returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            return !(_alreadyLowered.Contains(coords));
        }

        /**
         * <summary>
         * Update method for Hexes adjacent to Coords that were altered in
         * this round of expansion, but not expanded from themselves.
         * </summary>
         * <remarks>
         * For this class, we just clear the already-lowered hex tracking
         * for the next round of expansion.
         * </remarks>
         */
        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // Just clear out already raised hexes for the next round
            // of expansion
            _alreadyLowered.Clear();
        }

        /**
         * <summary>
         * Hex modification. Attempts to lower the HumidityLevel of the Hex
         * at given Coords by one step. See <see cref="HexMap.Aridify(Coords)"/>.
         * </summary>
         * <returns>True if the HumidityLevel was decreased, false otherwise.</returns>
         */
        protected override bool ModHex(Coords coords)
        {
            if (_map.Aridify(coords))
            {
                _alreadyLowered.Add(coords);
                return true;
            }
            return false;
        }
    }
}
