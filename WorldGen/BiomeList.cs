using System;
using System.Collections.Generic;
using System.IO;
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
        private static RandomGen rand = new RandomGen();
        private Stream _defsStream;

        private BiomeList()
        {
            Biomes = new List<Biome>();
            _defsStream = new FileStream("biome_defs.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            BiomeSetup();
        }

        public BiomeList(Stream stream)
        {
            Biomes = new List<Biome>();
            _defsStream = stream;
            BiomeSetup();
        }

        /**
         * <summary>
         * Method to setup biomes, as read from the biomes definition stream.
         * </summary>
         */
        private void BiomeSetup()
        {
            
            XDocument biomeDefs = XDocument.Load(_defsStream);
            XElement defsRoot = biomeDefs.Element("root");
            var temperatureMax = Enum.GetValues(typeof(Hex.TemperatureLevel)).Cast<int>().Last();
            var humidityMax = Enum.GetValues(typeof(Hex.HumidityLevel)).Cast<int>().Last();
            foreach (XElement node in defsRoot.Elements())
            {
                // check for disabling attribute, skip this node if it's set to true
                XAttribute disabledAttr = node.Attribute("disabled");
                if (disabledAttr != null && disabledAttr.Value == "true")
                    continue;

                // validate it has the minimum information
                if (node.Element("name") == null
                    || node.Element("hex_color") == null
                    || node.Element("primary_aspect") == null
                    || node.Element("temperature_lb") == null
                    || node.Element("temperature_ub") == null
                    || node.Element("humidity_lb") == null
                    || node.Element("humidity_ub") == null)
                {
                    continue;
                }

                string biomeName = node.Element("name").Value;
                string biomeColorStr = node.Element("hex_color").Value;

                // Can't deal with empty name or color, abandon ship
                if (biomeName.Length == 0 || biomeColorStr.Length == 0)
                    continue;

                // Regardless of how long the color string is, we'll only process 3 bytes' worth.
                // Beyond that, the string has no meaning.
                int numChars = Math.Min(biomeColorStr.Length, 6);
                byte[] colorBytes = new byte[3];
                try
                {
                    for (int i = 0; i < numChars; i += 2)
                        colorBytes[i / 2] = Convert.ToByte(biomeColorStr.Substring(i, 2), 16);
                }
                catch
                {
                    // Bail if the format conversion fails
                    continue;
                }

                Color biomeColor = Color.FromArgb(0xff, colorBytes[0], colorBytes[1], colorBytes[2]);
                AffinityMap biomeAffinities = new AffinityMap();

                try
                {
                    string primaryAspect = node.Element("primary_aspect").Value;
                    biomeAffinities.MaximizeAffinity(primaryAspect);
                }
                catch (InvalidAspectException)
                {
                    // Bail if this is not a valid primary aspect
                    continue;
                }

                Biome newBiome = new Biome(biomeName, biomeColor, biomeAffinities);

                try
                {
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
                }
                catch
                {
                    // Bail on any parsing exceptions
                    continue;
                }

                Biomes.Add(newBiome);
            }
        }

        public Biome SelectBiome(Hex.TemperatureLevel temperature, Hex.HumidityLevel humidity)
        {
            var validBiomes = Biomes.Where(x => x.TemperatureRange.Contains(temperature)
                                                && x.HumidityRange.Contains(humidity));
            int i = rand.GenerateInt(validBiomes.Count());
            return validBiomes.ElementAt(i);
        }
    }
}
