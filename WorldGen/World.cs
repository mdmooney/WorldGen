using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    class World
    {
        public HexMap Map { get; }
        private Dictionary<Coords, Demographics> _demographics;

        private int _width;
        private int _height;

        public World(int width, int height)
        {
            _width = width;
            _height = height;
            Map = new HexMap(width, height);
            _demographics = new Dictionary<Coords, Demographics>();
        }

        public Demographics DemographicsAt(Coords coords)
        {
            if (!_demographics.ContainsKey(coords)) return null;
            return (_demographics[coords]);
        }

        public void PlaceRaceAt(Race race, Coords coords)
        {
            // TODO: coords validation
            if (!_demographics.ContainsKey(coords))
                _demographics[coords] = new Demographics();
            else if (_demographics[coords].ContainsRace(race))
                return;

            _demographics[coords].IncreasePopulationFor(race);
        }

        public Color PopColorAt(Coords coords)
        {
            if (_demographics.ContainsKey(coords))
                return Colors.Red;

            return Map.BaseColorAt(coords);
        }
    }
}
