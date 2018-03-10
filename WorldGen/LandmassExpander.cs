using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class LandmassExpander : HexExpander
    {

        public LandmassExpander(HexMap map) : base(map)
        {}

        protected override bool CanExpandTo(Coords coords)
        {
            return _map.CanPlaceAt(coords);
        }

        protected override bool ModHex(Coords coords)
        {
            return _map.PlaceLand(coords);
        }

        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            _map.SetPlaceable(coords, false);
        }
    }
}
