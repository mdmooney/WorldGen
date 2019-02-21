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
    public class AffinityMapTests
    {
        // Simple setting and getting of affinities
        [TestMethod()]
        public void Set_Get_Affinity()
        {
            string testAspect = "fire";
            var am = new AffinityMap();

            // ensure we start with an empty map
            Assert.AreEqual(0, am.Count);
            Assert.IsFalse(am.AspectList.Contains(testAspect));

            // Get an aspect not in the map (should be 0)
            Assert.AreEqual(0, am.GetAffinity(testAspect));

            // set an affinity, validate that we add it (and only it)
            am.SetAffinity(testAspect, 1);
            Assert.AreEqual(1, am.Count);
            Assert.IsTrue(am.AspectList.Contains(testAspect));

            // validate that it has the right value
            Assert.AreEqual(1, am.GetAffinity(testAspect));

            // rewriting
            am.SetAffinity(testAspect, -1);
            Assert.AreEqual(1, am.Count);
            Assert.AreEqual(-1, am.GetAffinity(testAspect));
        }

        [TestMethod()]
        public void Min_Max_Affinity()
        {
            string testAspect = "fire";
            var am = new AffinityMap();

            // upper bounding
            am.SetAffinity(testAspect, 10);
            Assert.AreEqual(AffinityMap.MaxAffinity, am.GetAffinity(testAspect));

            // lower bounding
            am.SetAffinity(testAspect, -10);
            Assert.AreEqual(AffinityMap.MinAffinity, am.GetAffinity(testAspect));

            // zeroing
            am.SetAffinity(testAspect, 0);
            Assert.AreEqual(0, am.GetAffinity(testAspect));

            // maximize
            am.MaximizeAffinity(testAspect);
            Assert.AreEqual(AffinityMap.MaxAffinity, am.GetAffinity(testAspect));

            // minimize
            am.MinimizeAffinity(testAspect);
            Assert.AreEqual(AffinityMap.MinAffinity, am.GetAffinity(testAspect));
        }

        [TestMethod()]
        public void Intersect_Affinities()
        {
            List<string> aspects = new List<string>() { "earth", "air", "fire" };
            var am = new AffinityMap();
            am.MaximizeAffinity("earth");
            am.MinimizeAffinity("air");
            am.MaximizeAffinity("fire");
            am.MinimizeAffinity("water");

            var intersected = am.IntersectAspects(aspects);
            Assert.IsTrue(intersected.Contains("earth"));
            Assert.IsTrue(intersected.Contains("air"));
            Assert.IsTrue(intersected.Contains("fire"));
            Assert.IsFalse(intersected.Contains("water"));

        }

        [TestMethod()]
        public void Set_Get_Invalid_Affinity()
        {
            string invalidAspect = "this-is-not-a-real-aspect";
            var am = new AffinityMap();

            // ensure we start with an empty map
            Assert.AreEqual(0, am.Count);
            Assert.IsFalse(am.AspectList.Contains(invalidAspect));

            // attempting to add this to the map should throw
            Assert.ThrowsException<InvalidAspectException>(() => am.SetAffinity(invalidAspect, 1));

            // attempting to get it should do the same
            Assert.ThrowsException<InvalidAspectException>(() => am.GetAffinity(invalidAspect));
        }

        [TestMethod()]
        public void Combine_Affinities()
        {
            // Two max affinities => max combined
            int max = AffinityMap.MaxAffinity;
            int min = AffinityMap.MinAffinity;
            int val1 = max;
            int val2 = max;
            Assert.AreEqual(AffinityMap.MaxAffinity, AffinityMap.CombineAffinities(val1, val2));

            // Two min affities => max combined
            val1 = min;
            val2 = min;
            Assert.AreEqual(AffinityMap.MaxAffinity, AffinityMap.CombineAffinities(val1, val2));

            // Min and max affinities => min combined
            val1 = max;
            Assert.AreEqual(AffinityMap.MinAffinity, AffinityMap.CombineAffinities(val1, val2));

            // Ditto above, but reversed
            val1 = -max;
            val2 = max;
            Assert.AreEqual(AffinityMap.MinAffinity, AffinityMap.CombineAffinities(val1, val2));

            // One value 0, one not => 0 combined
            val1 = 0;
            Assert.AreEqual(0, AffinityMap.CombineAffinities(val1, val2));

            // One max, one half-max => half max combined
            val1 = max;
            val2 = max / 2;
            Assert.AreEqual(max / 2, AffinityMap.CombineAffinities(val1, val2));
        }

        [TestMethod()]
        public void Combine_Maps()
        {
            int max = AffinityMap.MaxAffinity;
            int min = AffinityMap.MinAffinity;
            AffinityMap a = new AffinityMap();
            AffinityMap b = new AffinityMap();

            // first try empty maps
            var newMap = a.CombineWith(b);
            Assert.AreEqual(newMap.Count, 0);

            // Populate the first map
            a.SetAffinity("fire", max);
            a.SetAffinity("water", min);
            a.SetAffinity("earth", max);
            a.SetAffinity("air", min);
            a.SetAffinity("desert", max);
            a.SetAffinity("mountain", max / 2);

            // combine populated map with empty map (should yield empty map)
            newMap = a.CombineWith(b);
            Assert.AreEqual(newMap.Count, 0); 

            // Populate the second map
            b.SetAffinity("fire", max);
            b.SetAffinity("water", min);
            b.SetAffinity("earth", min);
            b.SetAffinity("air", max);
            b.SetAffinity("forest", max);
            b.SetAffinity("mountain", max / 2);

            // Combine
            newMap = a.CombineWith(b);
            Assert.AreEqual(4, newMap.Count);

            // Validate
            Assert.AreEqual(max, newMap.GetAffinity("fire"));
            Assert.AreEqual(max, newMap.GetAffinity("water"));
            Assert.AreEqual(min, newMap.GetAffinity("earth"));
            Assert.AreEqual(min, newMap.GetAffinity("air"));
            Assert.IsFalse(newMap.AspectList.Contains("mountain"));
        }

        [TestMethod()]
        public void Get_Similarity()
        {
            int max = AffinityMap.MaxAffinity;
            int min = AffinityMap.MinAffinity;
            AffinityMap a = new AffinityMap();
            AffinityMap b = new AffinityMap();

            // Get similarity of empty maps (should be 0)
            int sim = a.GetSimilarityTo(b);
            Assert.AreEqual(0, sim);

            // Populate the first map
            a.SetAffinity("fire", max);
            a.SetAffinity("water", min);
            a.SetAffinity("earth", max);
            a.SetAffinity("air", min);
            a.SetAffinity("desert", max);
            a.SetAffinity("mountain", max / 2);

            // Similarity of populated map to empty map (still 0)
            sim = a.GetSimilarityTo(b);
            Assert.AreEqual(0, sim);

            // Populate the second map
            b.SetAffinity("fire", max);
            b.SetAffinity("water", min);
            b.SetAffinity("earth", min);
            b.SetAffinity("air", max);
            b.SetAffinity("forest", max);
            b.SetAffinity("mountain", max / 2);

            // Get similarity
            int expected = (max * max);
            expected -= (min * min);
            expected += (max / 2) * (max / 2) / 2;
            sim = a.GetSimilarityTo(b);
            Assert.AreEqual(expected, sim);

            // Add unshared aspect, should not change similarity
            a.SetAffinity("tundra", 3);
            sim = a.GetSimilarityTo(b);
            Assert.AreEqual(expected, sim);
        }

        [TestMethod()]
        public void To_String()
        {
            // empty map
            AffinityMap am = new AffinityMap();
            string str = am.ToString();
            string expected = "";
            Assert.AreEqual(expected, str);

            // 1-element map
            am.SetAffinity("fire", 3);
            str = am.ToString();
            string line1 = "fire : 3";
            expected = line1 + '\n';
            Assert.AreEqual(expected, str);

            // 2-element map
            am.SetAffinity("water", -3);
            str = am.ToString();
            string line2 = "water : -3";
            expected += line2 + '\n';
            Assert.AreEqual(expected, str);

            // 3-element map (test for alphabetic sorting)
            am.SetAffinity("mountain", 4);
            str = am.ToString();
            string line3 = "mountain : 4";
            expected = line1 + '\n' + line2 + '\n' + line3 + '\n';
            Assert.AreEqual(expected, str);
        }

    }
}