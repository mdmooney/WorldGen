using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen.Tests
{
    [TestClass()]
    public class BiomeListTests
    {
        [TestMethod()]
        public void GetInstanceTest()
        {
            BiomeList bl = BiomeList.GetInstance();
            Assert.IsNotNull(bl);
            BiomeList bl2 = BiomeList.GetInstance();
            Assert.AreSame(bl, bl2);
        }

        [TestMethod()]
        public void SetupTest()
        {
            BiomeList bl = BiomeList.GetInstance();
            Assert.IsTrue(bl.Biomes.Count > 0);
            var match = bl.Biomes.Where(x => x.Name == "tundra").ToList();
            Assert.IsTrue(match.Count == 1);
            match = bl.Biomes.Where(x => x.Name == "shmundra").ToList();
            Assert.IsTrue(match.Count == 0);
        }

        [TestMethod()]
        public void SelectBiomeTest()
        {
            Assert.Fail();
        }
    }
}