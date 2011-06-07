using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class UtilTests
    {
        [Test]
        public void test_split_array()
        {
            var arr = new byte[] { 1, 2, 3, 0, 4, 5, 0 };

            var arrs = Util.SplitArray(arr);

            Assert.IsNotNull(arrs);
            Assert.IsNotEmpty(arrs);

            var splits = "1230450".Split(new char[] {'0'});
        }

        [Test]
        public void split_array_will_return_two_empty_arrays_if_argument_only_contains_the_split_byte()
        {
            var arr = new byte[] { 0 };

            var arrs = Util.SplitArray(arr);

            Assert.IsNotNull(arrs);
            Assert.AreEqual(2, arrs.Length);
            Assert.AreEqual(new byte[] {}, arrs[0]);
            Assert.AreEqual(new byte[] {}, arrs[1]);
        }

        [Test]
        public void split_array_returns_array_with_entire_argument_in_array_if_argument_array_contains_no_split_bytes()
        {
            var arr = new byte[] { 1, 2, 3 };
            var arrs = Util.SplitArray(arr);

            Assert.IsNotNull(arrs);
            Assert.AreEqual(1, arrs.Length);
            Assert.AreEqual(arr, arrs[0]);
        }

        [Test]
        public void split_array_returns_array_with_empty_array_in_it_if_argument_array_is_empty()
        {
            var arr = new byte[] { };
            var arrs = Util.SplitArray(arr);

            Assert.IsNotNull(arrs);
            Assert.AreEqual(1, arrs.Length);
            Assert.AreEqual(arr, arrs[0]);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(ArgumentNullException))]
        public void split_array_throws_if_argument_is_null()
        {
            var arrs = Util.SplitArray(null);
        }
    }
}
