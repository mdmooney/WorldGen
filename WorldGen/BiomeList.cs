using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace WorldGen
{
    class BiomeList
    {
        public List<Biome> Biomes { get; private set; }
        private static BiomeList _instance;
        private static Random rand = new Random();

        private BiomeList()
        {
            Biomes = new List<Biome>();
            BiomeSetup();
        }

        public static BiomeList GetInstance()
        {
            if (_instance == null)
                _instance = new BiomeList();
            return _instance;
        }

        /**
         * <summary>
         * Method to setup biomes, as read from the biomes definition file (currently hardcoded
         * to the CWD, with the name "biome_defs.xml").
         * </summary>
         */
        private void BiomeSetup()
        {
            
            XDocument biomeDefs = XDocument.Load("biome_defs.xml");
            XElement defsRoot = biomeDefs.Element("root");
            var temperatureMax = Enum.GetValues(typeof(Hex.TemperatureLevel)).Cast<int>().Last();
            var humidityMax = Enum.GetValues(typeof(Hex.HumidityLevel)).Cast<int>().Last();
            foreach (XElement node in defsRoot.Elements())
            {
                // check for disabling attribute, skip this node if it's set to true
                XAttribute disabledAttr = node.Attribute("disabled");
                if (disabledAttr != null && disabledAttr.Value == "true")
                    continue;

                string biomeName = node.Element("name").Value;
                string biomeColorStr = node.Element("hex_color").Value;

                // Regardless of how long the color string is, we'll only process 3 bytes' worth.
                // Beyond that, the string has no meaning.
                int numChars = Math.Min(biomeColorStr.Length, 6);
                byte[] colorBytes = new byte[3];
                for (int i = 0; i < numChars; i += 2)
                    colorBytes[i / 2] = Convert.ToByte(biomeColorStr.Substring(i, 2), 16);

                Color biomeColor = Color.FromArgb(0xff, colorBytes[0], colorBytes[1], colorBytes[2]); 
                Biome newBiome = new Biome(biomeName, biomeColor);
                
                int temperatureLowerBound = Math.Max(int.Parse(node.Element("temperature_lb").Value), 0);
                int temperatureUpperBound = Math.Min(int.Parse(node.Element("temperature_ub").Value), temperatureMax);
                for (int i = temperatureLowerBound; i <= temperatureUpperBound; i++)
                {
                    newBiome.AddTemperature((Hex.TemperatureLevel)i);
                }

                int humidityLowerBound = Math.Max(int.Parse(node.Element("humidity_lb").Value), 0);
                int humidityUpperBound = Math.Min(int.Parse(node.Element("humidity_ub").Value), humidityMax);
                for (int i = humidityLowerBound; i <= humidityUpperBound; i++)
                {
                    newBiome.AddHumidity((Hex.HumidityLevel)i);
                }

                string primaryAspect = node.Element("primary_aspect").Value;
                newBiome.Affinities.MaximizeAffinity(primaryAspect);

                Biomes.Add(newBiome);
            }
        }

        public Biome SelectBiome(Hex.TemperatureLevel temperature, Hex.HumidityLevel humidity)
        {
            var validBiomes = Biomes.Where(x => x.TemperatureRange.Contains(temperature)
                                                && x.HumidityRange.Contains(humidity));
            int i = rand.Next(validBiomes.Count());
            return validBiomes.ElementAt(i);
        }
    }
}
