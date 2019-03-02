using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class HumidityExpander : HeightExpander
    {
        /**
         * <summary>
         * Constructor for HumidityExpander takes the map to alter.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         */
        public HumidityExpander(IRandomGen rand, HexMap map) : base(rand, map) { }

        /**
         * <summary>
         * Hex modification method for HumidityExpander attempts to lower the
         * HumidityLevel of the Hex at given Coords by one step. See
         * <see cref="HexMap.Aridify(Coords)"/>.
         * </summary>
         * <param name="coords">Coords of the Hex to raise.</param>
         */
        protected override bool LayeredModHex(Coords coords)
        {
            return _map.Aridify(coords);
        }
    }
}
