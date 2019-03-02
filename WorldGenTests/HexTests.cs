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
    public class HexTests
    {
        [TestMethod()]
        public void RotateSideCounterclockwiseTest()
        {
            var s = Hex.Side.North;
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.Northwest);
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.Southwest);
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.South);
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.Southeast);
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.Northeast);
            s = Hex.RotateSideCounterclockwise(s);
            Assert.AreEqual(s, Hex.Side.North);
        }

        [TestMethod()]
        public void RotateSideClockwiseTest()
        {
            var s = Hex.Side.North;
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.Northeast);
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.Southeast);
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.South);
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.Southwest);
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.Northwest);
            s = Hex.RotateSideClockwise(s);
            Assert.AreEqual(s, Hex.Side.North);
        }

        [TestMethod()]
        public void OppositeSideTest()
        {
            Assert.AreEqual(Hex.Side.South, Hex.OppositeSide(Hex.Side.North));
            Assert.AreEqual(Hex.Side.North, Hex.OppositeSide(Hex.Side.South));
            Assert.AreEqual(Hex.Side.Northeast, Hex.OppositeSide(Hex.Side.Southwest));
            Assert.AreEqual(Hex.Side.Southwest, Hex.OppositeSide(Hex.Side.Northeast));
            Assert.AreEqual(Hex.Side.Northwest, Hex.OppositeSide(Hex.Side.Southeast));
            Assert.AreEqual(Hex.Side.Southeast, Hex.OppositeSide(Hex.Side.Northwest));
        }

        [TestMethod()]
        public void IsLandOrWaterTest()
        {
            var h = new Hex();
            Assert.IsTrue(h.IsWater());
            Assert.IsFalse(h.IsLand());

            h = new Hex(Hex.HexType.Ocean);
            Assert.IsTrue(h.IsWater());
            Assert.IsFalse(h.IsLand());

            h = new Hex(Hex.HexType.Shore);
            Assert.IsTrue(h.IsWater());
            Assert.IsFalse(h.IsLand());

            h = new Hex(Hex.HexType.Land);
            Assert.IsFalse(h.IsWater());
            Assert.IsTrue(h.IsLand());

            var r = new River();
            var rs = new RiverSegment(r);
            h.MainRiverSegment = rs;
            Assert.IsFalse(h.IsWater());
            Assert.IsTrue(h.IsLand());
        }

        [TestMethod()]
        public void HasRiverTest()
        {
            var r = new River();
            var rs = new RiverSegment(r);
            var h =  new Hex();
            Assert.IsFalse(h.HasRiver());

            h.MainRiverSegment = rs;
            Assert.IsTrue(h.HasRiver());
        }
    }
}