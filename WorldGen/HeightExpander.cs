using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class HeightExpander : HexExpander
    {
        private Hex.Elevation _elevation;

        public HeightExpander(HexMap map, int pass) : base(map)
        {
            _elevation = (Hex.Elevation)pass;
            Console.WriteLine("Elevation: " + _elevation);
        }

        protected override bool CanExpandTo(Coords coords)
        {
            return _map.GetElevationAt(coords) < _elevation;
        }

        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // no implementation at this point
            // may add smoothing operation here of some kind
        }

        protected override bool ModHex(Coords coords)
        {
            if (!CanExpandTo(coords)) return false;
            return (_map.Raise(coords));
        }
    }
}
