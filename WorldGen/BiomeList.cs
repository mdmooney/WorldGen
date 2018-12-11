using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
         * Method to setup hardcoded biomes.
         * This will eventually be replaced by a data file of some kind.
         * </summary>
         */
        private void BiomeSetup()
        {
            Biome tundra = new Biome("tundra", Colors.Azure);
            tundra.AddTemperature(Hex.TemperatureLevel.Cold);
            tundra.AddHumidity(Hex.HumidityLevel.Arid);
            tundra.AddHumidity(Hex.HumidityLevel.SemiArid);
            tundra.AddHumidity(Hex.HumidityLevel.Average);
            Biomes.Add(tundra);

            Biome conifers = new Biome("coniferous forest", Colors.LightSeaGreen);
            conifers.AddTemperature(Hex.TemperatureLevel.Cold);
            conifers.AddTemperature(Hex.TemperatureLevel.Cool);
            conifers.AddHumidity(Hex.HumidityLevel.Average);
            conifers.AddHumidity(Hex.HumidityLevel.SemiHumid);
            conifers.AddHumidity(Hex.HumidityLevel.Humid);
            Biomes.Add(conifers);

            Biome shrubland = new Biome("shrubland", Colors.Yellow);
            shrubland.AddTemperature(Hex.TemperatureLevel.Cool);
            shrubland.AddTemperature(Hex.TemperatureLevel.Temperate);
            shrubland.AddTemperature(Hex.TemperatureLevel.Warm);
            shrubland.AddHumidity(Hex.HumidityLevel.Arid);
            shrubland.AddHumidity(Hex.HumidityLevel.SemiArid);
            Biomes.Add(shrubland);

            Biome grassland = new Biome("grassland", Colors.GreenYellow);
            grassland.AddTemperature(Hex.TemperatureLevel.Cool);
            grassland.AddTemperature(Hex.TemperatureLevel.Temperate);
            grassland.AddTemperature(Hex.TemperatureLevel.Warm);
            grassland.AddHumidity(Hex.HumidityLevel.SemiArid);
            grassland.AddHumidity(Hex.HumidityLevel.Average);
            Biomes.Add(grassland);

            Biome broadleafs = new Biome("broadleaf forest", Colors.ForestGreen);
            broadleafs.AddTemperature(Hex.TemperatureLevel.Cool);
            broadleafs.AddTemperature(Hex.TemperatureLevel.Temperate);
            broadleafs.AddTemperature(Hex.TemperatureLevel.Warm);
            broadleafs.AddHumidity(Hex.HumidityLevel.Average);
            broadleafs.AddHumidity(Hex.HumidityLevel.SemiHumid);
            broadleafs.AddHumidity(Hex.HumidityLevel.Humid);
            Biomes.Add(broadleafs);

            Biome desert = new Biome("desert", Colors.Orange);
            desert.AddTemperature(Hex.TemperatureLevel.Hot);
            desert.AddTemperature(Hex.TemperatureLevel.Warm);
            desert.AddHumidity(Hex.HumidityLevel.Arid);
            desert.AddHumidity(Hex.HumidityLevel.SemiArid);
            Biomes.Add(desert);

            Biome savanna = new Biome("savanna", Colors.Gold);
            savanna.AddTemperature(Hex.TemperatureLevel.Hot);
            savanna.AddHumidity(Hex.HumidityLevel.Average);
            savanna.AddHumidity(Hex.HumidityLevel.SemiHumid);
            savanna.AddHumidity(Hex.HumidityLevel.SemiArid);
            Biomes.Add(savanna);

            Biome jungle = new Biome("jungle", Colors.SpringGreen);
            jungle.AddTemperature(Hex.TemperatureLevel.Hot);
            jungle.AddHumidity(Hex.HumidityLevel.SemiHumid);
            jungle.AddHumidity(Hex.HumidityLevel.Humid);
            Biomes.Add(jungle);

            Biome woodland = new Biome("woodland", Colors.OliveDrab);
            woodland.AddTemperature(Hex.TemperatureLevel.Temperate);
            woodland.AddTemperature(Hex.TemperatureLevel.Warm);
            woodland.AddHumidity(Hex.HumidityLevel.SemiArid);
            woodland.AddHumidity(Hex.HumidityLevel.SemiHumid);
            woodland.AddHumidity(Hex.HumidityLevel.Average);
            Biomes.Add(woodland);

            Biome swamp = new Biome("swamp", Colors.Brown);
            swamp.AddTemperature(Hex.TemperatureLevel.Hot);
            swamp.AddTemperature(Hex.TemperatureLevel.Warm);
            swamp.AddHumidity(Hex.HumidityLevel.SemiHumid);
            swamp.AddHumidity(Hex.HumidityLevel.Humid);
            //Biomes.Add(swamp);

            Biome taiga = new Biome("taiga", Colors.Teal);
            taiga.AddTemperature(Hex.TemperatureLevel.Cold);
            taiga.AddHumidity(Hex.HumidityLevel.Humid);
            taiga.AddHumidity(Hex.HumidityLevel.SemiHumid);
            Biomes.Add(taiga);
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
