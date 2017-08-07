using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miktemk.ZUnitTests
{
    public class UnitTestUtils
    {
        public static void AssertEnumerablesAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> compareElements)
        {
            var arrAct = actual.ToArray();
            Assert.AreEqual(expected.Count(), actual.Count());
            expected.EnumerateWith(actual, (exp, act, index) => {
                Assert.AreEqual(true, compareElements(exp, act));
            });
        }

        public static void AssertEnumerablesOfEnumerablesAreEqual<T>(
            IEnumerable<IEnumerable<T>> expected,
            IEnumerable<IEnumerable<T>> actual,
            Func<T, T, bool> compareElements)
        {
            AssertEnumerablesAreEqual(expected, actual, (subExp, subAct) =>
            {
                AssertEnumerablesAreEqual(subExp, subAct, (e, a) => compareElements(e, a));
                return true;
            });
        }

        public static string DebugStrigFromEnumSplitted(IEnumerable<IEnumerable<char>> result)
        {
            return result.Select(x => new string(x.ToArray())).StringJoin(",");
        }
    }
}
