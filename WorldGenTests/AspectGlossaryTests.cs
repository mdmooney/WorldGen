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
    public class AspectGlossaryTests
    {
        [TestMethod()]
        public void GetInstanceTest()
        {
            AspectGlossary instance = AspectGlossary.GetInstance();
            Assert.IsNotNull(instance);
            AspectGlossary instance2 = AspectGlossary.GetInstance();
            Assert.AreSame(instance, instance2);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            AspectGlossary instance = AspectGlossary.GetInstance();
            Assert.IsTrue(instance.Contains("fire"));
            Assert.IsTrue(instance.Contains("fIrE"));
            Assert.IsTrue(instance.Contains("FIRE"));
            Assert.IsFalse(instance.Contains("shmire"));
            Assert.IsFalse(instance.Contains(""));
            Assert.IsFalse(instance.Contains(null));
        }

        [TestMethod()]
        public void GetPoolTest()
        {
            AspectGlossary instance = AspectGlossary.GetInstance();
            var pool = instance.GetPool("geography");
            Assert.IsTrue(pool.Count > 0);
            Assert.IsTrue(pool.Contains("mountain"));
            Assert.IsFalse(pool.Contains("fire"));
            Assert.ThrowsException<InvalidAspectException>(() => instance.GetPool("shmeography"));
            Assert.ThrowsException<InvalidAspectException>(() => instance.GetPool(""));
            Assert.ThrowsException<InvalidAspectException>(() => instance.GetPool(null));
        }

        [TestMethod()]
        public void HasPoolTest()
        {
            AspectGlossary instance = AspectGlossary.GetInstance();

            // Pass cases (case insensitive)
            Assert.IsTrue(instance.HasPool("geography"));
            Assert.IsTrue(instance.HasPool("GEOGRAPHY"));
            Assert.IsTrue(instance.HasPool("geOGrapHy"));

            // Fail cases
            Assert.IsFalse(instance.HasPool("shmeography"));
            Assert.IsFalse(instance.HasPool(""));
            Assert.IsFalse(instance.HasPool(null));
        }
    }
}
