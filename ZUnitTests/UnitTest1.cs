using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookAligner.Code;
using Miktemk.ZUnitTests;
using Miktemk;

namespace ZUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_SplitSection()
        {
            // .... "the box" play with it to test it out
            const int OFFSET = 200;
            const int LENGTH = 100;
            var sectionAll = new DocSection(OFFSET, LENGTH);
            var highlights = new WordHighlight[] {
                new WordHighlight(OFFSET + 10, 1),
                new WordHighlight(OFFSET + 20, 1),
                new WordHighlight(OFFSET + 30, 1),
                new WordHighlight(OFFSET + 40, 1),
                new WordHighlight(OFFSET + 50, 1),
                new WordHighlight(OFFSET + 50, 1),
                new WordHighlight(OFFSET + 60, 1),
                new WordHighlight(OFFSET + 70, 1),
                new WordHighlight(OFFSET + 80, 1),
                new WordHighlight(OFFSET + 90, 1),
            };
            var splits = Utils.SplitSection(sectionAll, highlights);

            // .... length as expected?
            Assert.AreEqual(highlights.Length, splits.Count());
            // .... every char accounted for?
            Assert.AreEqual(LENGTH, splits.Select(x => x.Length).Sum());
            // .... all splits inside the "box"?
            foreach (var split in splits)
            {
                Assert.IsTrue(split.StartIndex >= OFFSET);
                Assert.IsTrue(split.StartIndex + split.Length <= OFFSET + LENGTH);
            }
        }

        [TestMethod]
        public void Test_PositionAlignmentAlgorithm()
        {
            var algo = new AlgorithmArrayAlign();
            var arr1 = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            // .... same
            var arr2 = arr1.ToArray();
            var result = algo.GetBestAlignment(arr1, arr2, 10);
            var expect = arr1.Select((x, i) => new ArrayAlignIndexPair(i, i)).ToArray();
            AssertArrayAlignItemsAreEqual(expect, result);

            // .... +1
            arr2 = arr1.Select(x => x + 1).ToArray();
            result = algo.GetBestAlignment(arr1, arr2, 10);
            expect = arr1.Select((x, i) => new ArrayAlignIndexPair(i, i)).ToArray();
            AssertArrayAlignItemsAreEqual(expect, result);

            // .... +4
            arr2 = arr1.Select(x => x + 4).ToArray();
            result = algo.GetBestAlignment(arr1, arr2, 10);
            expect = arr1.Select((x, i) => new ArrayAlignIndexPair(i, i)).ToArray();
            AssertArrayAlignItemsAreEqual(expect, result);

            // .... no 70 or 80 in 2
            arr2 = arr1.Where(x => x != 70 && x != 80).ToArray();
            result = algo.GetBestAlignment(arr1, arr2, 5);
            expect = arr2.Select((x, i) => new ArrayAlignIndexPair(Array.IndexOf(arr1, x), i)).ToArray();
            AssertArrayAlignItemsAreEqual(expect, result);
        }

        private void AssertArrayAlignItemsAreEqual(ArrayAlignIndexPair[] expect, ArrayAlignIndexPair[] result)
        {
            UnitTestUtils.AssertEnumerablesAreEqual(expect, result, (x, y) => x.Index1 == y.Index1 && x.Index2 == y.Index2);
        }
    }
}
