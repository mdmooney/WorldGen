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
    public class HexMapTests
    {
        private MockRandomGen mr = new MockRandomGen();

        private HexMap CreateTestMap()
        {
            var rv = new HexMap(mr, 20, 20);
            return rv;
        }

        [TestMethod()]
        public void GetElevationAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMainRiverSegmentAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetTypeAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetTypeAtTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BaseColorAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ElevationColorAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TemperatureColorAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HumidityColorAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTemperatureAndHumidityAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBiomeAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetBiomeAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BiomeColorAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PlaceLandTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BordersOceanTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAdjacentOceanHexesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CanPlaceAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsRiverAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsLandAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsWaterAtTest()
        {
            var hm = CreateTestMap();
            var c = new Coords(0, 0);
            Assert.IsTrue(hm.IsWaterAt(c));

            hm.SetTypeAt(c, Hex.HexType.Shore);
            Assert.IsTrue(hm.IsWaterAt(c));

            hm.SetTypeAt(c, Hex.HexType.Land);
            Assert.IsFalse(hm.IsWaterAt(c));

            c = new Coords(5, 5);
            Assert.IsTrue(hm.IsWaterAt(c));
        }

        [TestMethod()]
        public void SetPlaceableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RaiseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AridifyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetTemperatureAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddMainRiverAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllAdjacentCoordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetFilteredAdjacencyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPlaceableAdjacentCoordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsAdjacentTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAdjacentSideTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllCoordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAdjacentCoordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAffinitiesForLandmassTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAffinitiesForCoordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetRandomLandmassTest()
        {
            Assert.Fail();
        }
    }
}