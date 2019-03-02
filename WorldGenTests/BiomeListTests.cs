using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldGen;

namespace WorldGen.Tests
{
    [TestClass()]
    public class BiomeListTests
    {
        private static MockRandomGen _rand = new MockRandomGen();

        [TestMethod()]
        public void SetupTest()
        {
            // Single biome definition
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "4", "0", "4");
            BiomeList bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 1);

            // Multiple biome definitions
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "00ff00", "water", "1", "3", "1", "3");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 2);

            // Multiple biome definitions, with duplicate name (should add fine)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "0000ff", "earth", "2", "3", "2", "3");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 3);

            // Multiple biome definitions, with a disabled biome (should not add)
            mbds.Reopen();
            mbds.AddDisabledBiomeDef("test biome 3", "0000ff", "earth", "2", "3", "2", "3");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 3);
        }

        [TestMethod()]
        public void SetupMalformedBiomesTest()
        {
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            BiomeList bl;

            // All tags, but invalid color string
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "tyrannosaurus", "fire", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid primary aspect
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "tyrannosaurus", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) lower temperature bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "tyrannosaurus", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) upper temperature bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "tyrannosaurus", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) lower humidity bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "4", "tyrannosaurus", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) upper humidity bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "4", "0", "tyrannosaurus");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // Malformed biome -- all tags, but temperature bounds out of range (should re-bound and add)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "-1", "6", "2", "3");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 1);
            var b = bl.Biomes.Find(x => x.Name == "test biome");
            Assert.IsTrue(b.TemperatureRange.Count == 5);
            Assert.IsTrue(b.HumidityRange.Count == 2);

            // Malformed biome -- all tags, but humidity bounds out of range (should re-bound and add)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "ff00ff", "fire", "2", "3", "-5", "7");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 2);
            b = bl.Biomes.Find(x => x.Name == "test biome 2");
            Assert.IsTrue(b.TemperatureRange.Count == 2);
            Assert.IsTrue(b.HumidityRange.Count == 5);
        }

        [TestMethod()]
        public void SetupIncompleteBiomesTest()
        {
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            BiomeList bl;

            // All tags, but empty name 
            mbds.AddBiomeDef("", "ff00ff", "fire", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but empty color string
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "", "fire", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // Nothing but opening/closing biome_def tags
            mbds.Reopen();
            mbds.AddInTag("biome_def", "");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No name value
            mbds.Reopen();
            mbds.AddBiomeDef(null, "ff00ff", "fire", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No color value
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", null, "fire", "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No primary aspect
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", null, "0", "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No temperature lower bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", null, "4", "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No temperature upper bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", null, "0", "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No humidity lower bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "4", null, "4");
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No humidity upper bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "4", "0", null);
            bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);
        }


        [TestMethod()]
        public void SelectBiomeTest()
        {
            // Add some biome definitions
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            mbds.AddBiomeDef("test biome", "ff0000", "fire", "0", "2", "0", "2");
            mbds.AddBiomeDef("test biome 2", "0000ff", "water", "3", "3", "3", "4");
            mbds.AddBiomeDef("test biome 3", "00ff00", "earth", "0", "2", "3", "4");
            mbds.AddBiomeDef("test biome 4", "ffffff", "air", "3", "4", "0", "2");
            BiomeList bl = new BiomeList(_rand, mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 4);

            var b = bl.SelectBiome(Hex.TemperatureLevel.Cold, Hex.HumidityLevel.Arid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cold, Hex.HumidityLevel.SemiArid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cold, Hex.HumidityLevel.Average);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cold, Hex.HumidityLevel.SemiHumid);
            Assert.IsTrue(b.Name == "test biome 3");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cold, Hex.HumidityLevel.Humid);
            Assert.IsTrue(b.Name == "test biome 3");

            b = bl.SelectBiome(Hex.TemperatureLevel.Cool, Hex.HumidityLevel.Arid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cool, Hex.HumidityLevel.SemiArid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cool, Hex.HumidityLevel.Average);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cool, Hex.HumidityLevel.SemiHumid);
            Assert.IsTrue(b.Name == "test biome 3");
            b = bl.SelectBiome(Hex.TemperatureLevel.Cool, Hex.HumidityLevel.Humid);
            Assert.IsTrue(b.Name == "test biome 3");

            b = bl.SelectBiome(Hex.TemperatureLevel.Temperate, Hex.HumidityLevel.Arid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Temperate, Hex.HumidityLevel.SemiArid);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Temperate, Hex.HumidityLevel.Average);
            Assert.IsTrue(b.Name == "test biome");
            b = bl.SelectBiome(Hex.TemperatureLevel.Temperate, Hex.HumidityLevel.SemiHumid);
            Assert.IsTrue(b.Name == "test biome 3");
            b = bl.SelectBiome(Hex.TemperatureLevel.Temperate, Hex.HumidityLevel.Humid);
            Assert.IsTrue(b.Name == "test biome 3");

            b = bl.SelectBiome(Hex.TemperatureLevel.Warm, Hex.HumidityLevel.Arid);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Warm, Hex.HumidityLevel.SemiArid);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Warm, Hex.HumidityLevel.Average);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Warm, Hex.HumidityLevel.SemiHumid);
            Assert.IsTrue(b.Name == "test biome 2");
            b = bl.SelectBiome(Hex.TemperatureLevel.Warm, Hex.HumidityLevel.Humid);
            Assert.IsTrue(b.Name == "test biome 2");

            b = bl.SelectBiome(Hex.TemperatureLevel.Hot, Hex.HumidityLevel.Arid);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Hot, Hex.HumidityLevel.SemiArid);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Hot, Hex.HumidityLevel.Average);
            Assert.IsTrue(b.Name == "test biome 4");
            b = bl.SelectBiome(Hex.TemperatureLevel.Hot, Hex.HumidityLevel.SemiHumid);
            Assert.IsNull(b);
            b = bl.SelectBiome(Hex.TemperatureLevel.Hot, Hex.HumidityLevel.Humid);
            Assert.IsNull(b);
        }
    }
}