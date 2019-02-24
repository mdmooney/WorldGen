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
    public class BiomeTests
    {
        [TestMethod()]
        public void AddTemperatureTest()
        {
            Biome testBiome = new Biome("test");
            Assert.AreEqual(0, testBiome.TemperatureRange.Count);
            testBiome.AddTemperature(Hex.TemperatureLevel.Hot);
            Assert.AreEqual(1, testBiome.TemperatureRange.Count);
            Assert.IsTrue(testBiome.TemperatureRange.Contains(Hex.TemperatureLevel.Hot));
            Assert.IsFalse(testBiome.TemperatureRange.Contains(Hex.TemperatureLevel.Cold));
            testBiome.AddTemperature(Hex.TemperatureLevel.Hot);
            Assert.AreEqual(1, testBiome.TemperatureRange.Count);
            testBiome.AddTemperature(Hex.TemperatureLevel.Cold);
            Assert.AreEqual(2, testBiome.TemperatureRange.Count);
            Assert.IsTrue(testBiome.TemperatureRange.Contains(Hex.TemperatureLevel.Hot));
            Assert.IsTrue(testBiome.TemperatureRange.Contains(Hex.TemperatureLevel.Cold));
        }

        [TestMethod()]
        public void AddHumidityTest()
        {
            Biome testBiome = new Biome("test");
            Assert.AreEqual(0, testBiome.HumidityRange.Count);
            testBiome.AddHumidity(Hex.HumidityLevel.Arid);
            Assert.AreEqual(1, testBiome.HumidityRange.Count);
            Assert.IsTrue(testBiome.HumidityRange.Contains(Hex.HumidityLevel.Arid));
            Assert.IsFalse(testBiome.HumidityRange.Contains(Hex.HumidityLevel.Humid));
            testBiome.AddHumidity(Hex.HumidityLevel.Arid);
            Assert.AreEqual(1, testBiome.HumidityRange.Count);
            testBiome.AddHumidity(Hex.HumidityLevel.Humid);
            Assert.AreEqual(2, testBiome.HumidityRange.Count);
            Assert.IsTrue(testBiome.HumidityRange.Contains(Hex.HumidityLevel.Humid));
            Assert.IsTrue(testBiome.HumidityRange.Contains(Hex.HumidityLevel.Arid));
        }

        [TestMethod()]
        public void TemperatureAndHumidityValidTest()
        {
            Biome testBiome = new Biome("test");
            var hot = Hex.TemperatureLevel.Hot;
            var cold = Hex.TemperatureLevel.Cold;
            var arid = Hex.HumidityLevel.Arid;
            var humid = Hex.HumidityLevel.Humid;

            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(hot, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, humid));

            testBiome.AddHumidity(arid);
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(hot, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, humid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(hot, humid));

            testBiome.AddTemperature(hot);
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(hot, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, humid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(hot, humid));

            testBiome.AddHumidity(humid);
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(hot, arid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, humid));
            Assert.IsFalse(testBiome.TemperatureAndHumidityValid(cold, arid));
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(hot, humid));

            testBiome.AddTemperature(cold);
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(hot, arid));
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(cold, humid));
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(cold, arid));
            Assert.IsTrue(testBiome.TemperatureAndHumidityValid(hot, humid));
        }
    }
}