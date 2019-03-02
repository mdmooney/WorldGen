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
    public class CoordsTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            Coords c1 = new Coords();
            Coords c2 = new Coords(0, 0);
            Assert.AreEqual(c1, c2);

            Coords c3 = new Coords(0, 1);
            Assert.AreNotEqual(c1, c3);
            c3 = new Coords(0, -1);
            Assert.AreNotEqual(c1, c3);
            c3 = new Coords(1, 0);
            Assert.AreNotEqual(c1, c3);
            c3 = new Coords(-1, 0);
            Assert.AreNotEqual(c1, c3);

            int n = 0;
            Assert.AreNotEqual(c1, n);
        }
    }
}