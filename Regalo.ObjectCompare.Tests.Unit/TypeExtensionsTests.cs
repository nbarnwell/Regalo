using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ObjectCompare.Tests.Unit
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        [TestCase(typeof(bool),    true)]
        [TestCase(typeof(byte),    true)]
        [TestCase(typeof(sbyte),   true)]
        [TestCase(typeof(char),    true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(double),  true)]
        [TestCase(typeof(float),   true)]
        [TestCase(typeof(int),     true)]
        [TestCase(typeof(uint),    true)]
        [TestCase(typeof(long),    true)]
        [TestCase(typeof(ulong),   true)]
        [TestCase(typeof(object),  true)]
        [TestCase(typeof(short),   true)]
        [TestCase(typeof(ushort),  true)]
        [TestCase(typeof(string),  true)]
        [TestCase(typeof(Type),    false)]
        public void IsPrimitiveTest(Type type, bool expectedResult)
        {
            var result = type.IsPrimitive();

            Assert.AreEqual(expectedResult, result, "Check for primitive type has failed.");
        }

        [Test]
        [TestCase(typeof(string[]), true)]
        [TestCase(typeof(string),   false)]
        [TestCase(typeof(Type),     false)]
        [TestCase(typeof(IList<>),  true)]
        public void IsEnumerableTest(Type type, bool expectedResult)
        {
            var result = type.IsEnumerable();

            Assert.AreEqual(expectedResult, result, "Check for enumerable type has failed.");
        }
    }
}