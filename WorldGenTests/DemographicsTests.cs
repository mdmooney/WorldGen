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
    public class DemographicsTests
    {
        private Race MakeTestRace(string name)
        {
            var n = new Name(name, name, name);
            var a = new AffinityMap(new MockRandomGen());
            return new Race(a, n);
        }

        [TestMethod()]
        public void IncreaseAndGetPopulationForTest()
        {
            var d = new Demographics();
            Assert.AreEqual(0, d.Populations.Count);
            var r = MakeTestRace("test race");
            d.IncreasePopulationFor(r);
            Assert.AreEqual(1, d.Populations.Count);
            Assert.AreEqual(Demographics.Population.Low, d.GetPopulationFor(r));
            d.IncreasePopulationFor(r);
            Assert.AreEqual(1, d.Populations.Count);
            Assert.AreEqual(Demographics.Population.LowMid, d.GetPopulationFor(r));

            var r2 = MakeTestRace("test race 2");
            d.IncreasePopulationFor(r2);
            Assert.AreEqual(Demographics.Population.Low, d.GetPopulationFor(r2));
            Assert.AreEqual(Demographics.Population.LowMid, d.GetPopulationFor(r));

            d.IncreasePopulationFor(r);
            d.IncreasePopulationFor(r);
            d.IncreasePopulationFor(r);
            Assert.AreEqual(Demographics.Population.High, d.GetPopulationFor(r));

            // Should not be able to increase population past high
            d.IncreasePopulationFor(r);
            Assert.AreEqual(Demographics.Population.High, d.GetPopulationFor(r));
            Assert.AreEqual(Demographics.Population.Low, d.GetPopulationFor(r2));
        }

        [TestMethod()]
        public void GetPopulationForTest()
        {
            var d = new Demographics();
            Assert.AreEqual(0, d.Populations.Count);
            var r = MakeTestRace("test race");
            var r2 = MakeTestRace("test race 2");
            Assert.AreEqual(Demographics.Population.None, d.GetPopulationFor(r));
            Assert.AreEqual(Demographics.Population.None, d.GetPopulationFor(r2));
            Assert.AreEqual(Demographics.Population.None, d[r]);
            Assert.AreEqual(Demographics.Population.None, d[r2]);
            d.IncreasePopulationFor(r);
            Assert.AreEqual(1, d.Populations.Count);
            Assert.AreEqual(Demographics.Population.Low, d.GetPopulationFor(r));
            Assert.AreEqual(Demographics.Population.None, d.GetPopulationFor(r2));
            Assert.AreEqual(Demographics.Population.Low, d[r]);
            Assert.AreEqual(Demographics.Population.None, d[r2]);

            d.IncreasePopulationFor(r);
            Assert.AreEqual(1, d.Populations.Count);
            Assert.AreEqual(Demographics.Population.LowMid, d.GetPopulationFor(r));
            Assert.AreEqual(Demographics.Population.None, d.GetPopulationFor(r2));
            Assert.AreEqual(Demographics.Population.LowMid, d[r]);
            Assert.AreEqual(Demographics.Population.None, d[r2]);
        }

        [TestMethod()]
        public void ContainsRaceTest()
        {
            var d = new Demographics();
            Assert.AreEqual(0, d.Populations.Count);
            var r = MakeTestRace("test race");
            var r2 = MakeTestRace("test race 2");
            Assert.IsFalse(d.ContainsRace(r));
            Assert.IsFalse(d.ContainsRace(r2));

            d.IncreasePopulationFor(r);
            Assert.IsTrue(d.ContainsRace(r));
            Assert.IsFalse(d.ContainsRace(r2));

            d.IncreasePopulationFor(r2);
            Assert.IsTrue(d.ContainsRace(r));
            Assert.IsTrue(d.ContainsRace(r2));
        }
    }
}
