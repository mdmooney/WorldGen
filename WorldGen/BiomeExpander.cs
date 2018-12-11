using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class BiomeExpander : HexExpander
    {
        private BiomeList _biomes;
        private Biome _currentBiome;

        public BiomeExpander(HexMap map) : base(map)
        {
            _biomes = BiomeList.GetInstance();
        }

        protected override bool CanExpandFirst(Coords coords)
        {
            return (_map.GetBiomeAt(coords) == null);
        }

        protected override bool CanExpandTo(Coords coords)
        {
            if (_map.GetBiomeAt(coords) != null)
                return false;
            if (_currentBiome == null)
                return true;

            var TempAndHumidity = _map.GetTemperatureAndHumidityAt(coords);
            return _currentBiome.TemperatureAndHumidityValid(TempAndHumidity.Item1,
                                                             TempAndHumidity.Item2);
        }

        protected override bool ModHex(Coords coords)
        {
            if (_currentBiome == null)
            {
                var TempAndHumidity = _map.GetTemperatureAndHumidityAt(coords);
                _currentBiome = _biomes.SelectBiome(TempAndHumidity.Item1, TempAndHumidity.Item2);
            }

            _map.SetBiomeAt(_currentBiome, coords);
            return true;
        }

        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // no op
        }
    }
}
