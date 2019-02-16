using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RaceGen
    {
        private HexMap _map;
        private Dictionary<string, RacePrototype> _prototypes;
        private XDocument _baseRaceDefs;

        public RaceGen(HexMap map)
        {
            _map = map;
            CreatePrototypes();
        }

        public RaceGen()
        {
            CreatePrototypes();
        }

        private void CreatePrototypes()
        {
            _prototypes = new Dictionary<string, RacePrototype>();
            _baseRaceDefs = XDocument.Load("base_race_defs.xml");
            XElement defsRoot = _baseRaceDefs.Element("root");
            foreach (XElement node in defsRoot.Elements())
            {
                Name name = new Name(node.Element("name_plr").Value,
                                     node.Element("name_sng").Value,
                                     node.Element("name_adj").Value);
                RacePrototype prototype = new RacePrototype(name);
                
                foreach (XElement affinityNode in node.Elements("aspect"))
                {
                    prototype.SetAffinity(affinityNode.Value, Int32.Parse(affinityNode.Attribute("affinity").Value));
                }

                foreach (XElement wildcardNode in node.Elements("wildcard"))
                {
                    prototype.SetWildcard(wildcardNode.Value, Int32.Parse(wildcardNode.Attribute("affinity").Value));
                }

                _prototypes.Add(name.Plural, prototype);
            }
        }

        public Race GenerateRace(AffinityMap massAffinity)
        {
            var races = new List<Race>();
            var raceTable = new RandomTable<Race>();
            foreach (var prototype in _prototypes.Values)
            {
                Race race = prototype.FinalizeAgainstMap(massAffinity);
                AffinityMap raceAffinity = race.Affinities;
                int score = race.Affinities.GetSimilarityTo(massAffinity);
                raceTable.Add(race, score);
            }

            Race rolledRace = raceTable.Roll();
            return rolledRace;
        }
    }
}
