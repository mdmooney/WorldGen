using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WorldGen.Tests
{
    [TestClass()]
    public class BiomeListTests
    {
        [TestMethod()]
        public void SetupTest()
        {
            // Single biome definition
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "5", "0", "5");
            BiomeList bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 1);

            // Multiple biome definitions
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "00ff00", "water", "1", "3", "1", "3");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 2);

            // Multiple biome definitions, with duplicate name (should add fine)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "0000ff", "earth", "2", "3", "2", "3");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 3);

            // Multiple biome definitions, with a disabled biome (should not add)
            mbds.Reopen();
            mbds.AddDisabledBiomeDef("test biome 3", "0000ff", "earth", "2", "3", "2", "3");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 3);
        }

        [TestMethod()]
        public void SetupMalformedBiomesTest()
        {
            MockBiomeDefStream mbds = new MockBiomeDefStream();
            BiomeList bl;

            // All tags, but invalid color string
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "tyrannosaurus", "fire", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid primary aspect
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "tyrannosaurus", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) lower temperature bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "tyrannosaurus", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) upper temperature bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "tyrannosaurus", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) lower humidity bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "5", "tyrannosaurus", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but invalid (unparseable) upper humidity bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "5", "0", "tyrannosaurus");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // Malformed biome -- all tags, but temperature bounds out of range (should re-bound and add)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "-1", "6", "2", "3");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 1);
            var b = bl.Biomes.Find(x => x.Name == "test biome");
            Assert.IsTrue(b.TemperatureRange.Count == 5);
            Assert.IsTrue(b.HumidityRange.Count == 2);

            // Malformed biome -- all tags, but humidity bounds out of range (should re-bound and add)
            mbds.Reopen();
            mbds.AddBiomeDef("test biome 2", "ff00ff", "fire", "2", "3", "-5", "7");
            bl = new BiomeList(mbds.FinalizeStream());
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
            mbds.AddBiomeDef("", "ff00ff", "fire", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // All tags, but empty color string
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "", "fire", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // Nothing but opening/closing biome_def tags
            mbds.Reopen();
            mbds.AddInTag("biome_def", "");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No name value
            mbds.Reopen();
            mbds.AddBiomeDef(null, "ff00ff", "fire", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No color value
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", null, "fire", "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No primary aspect
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", null, "0", "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No temperature lower bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", null, "5", "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No temperature upper bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", null, "0", "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No humidity lower bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "5", null, "5");
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);

            // No humidity upper bound
            mbds.Reopen();
            mbds.AddBiomeDef("test biome", "ff00ff", "fire", "0", "5", "0", null);
            bl = new BiomeList(mbds.FinalizeStream());
            Assert.IsTrue(bl.Biomes.Count == 0);
        }


        [TestMethod()]
        public void SelectBiomeTest()
        {
            Assert.Fail("Random testing not yet implemented.");
        }
    }
}