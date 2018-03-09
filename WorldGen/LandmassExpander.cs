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

        protected override int RollModifier()
        {
            double ratio = _remainingHexes / _totalHexes;
            Random rnd = new Random();
            int mod = 0;

            while ((mod < 6)
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

        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            _map.SetPlaceable(coords, false);
        }
    }
}
