using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class Demographics
    {
        public enum Population
        {
            Low,
            LowMid,
            Mid,
            MidHigh,
            High
        }

        private Dictionary<Race, Population> _populations;

        public Dictionary<Race, Population> Populations
        {
            get { return _populations; }
        }

        public Demographics()
        {
            _populations = new Dictionary<Race, Population>();
        }

        public void IncreasePopulationFor(Race race)
        {
            if (!_populations.ContainsKey(race))
                _populations.Add(race, Population.Low);
            else if (_populations[race] < Population.High)
            {
                _populations[race]++;
            }
        }

        public bool ContainsRace(Race race)
        {
            return _populations.ContainsKey(race);
        }

    }
}
