using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Regalo.ObjectCompare.Tests.Unit
{
    [TestFixture]
    public class ObjectComparerTests
    {
        private IObjectComparerProvider _objectComparerProvider;

        [SetUp]
        public void SetUp()
        {
            _objectComparerProvider = new ObjectComparerProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _objectComparerProvider = null;
        }

        [Test]
        public void CompareEmptyObjects()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInEmptyClass), _objectComparerProvider);
            var object1 = new NonBuiltInEmptyClass();
            var object2 = new NonBuiltInEmptyClass();

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result, "The objects are not equal, despite being empty.");
        }

        [Test]
        public void CompareNullWithAnObject()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInEmptyClass), _objectComparerProvider);
            var object1 = new NonBuiltInEmptyClass();

            bool result = comparer.AreEqual(object1, null);

            Assert.IsFalse(result, "Object reference and null are not equal.");
        }

        [Test]
        public void CompareNullWithNull()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInEmptyClass), _objectComparerProvider);

            bool result = comparer.AreEqual(null, null);

            Assert.IsTrue(result, "null reference and null are equal.");
        }

        [Test]
        public void CompareSameObjects()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInEmptyClass), _objectComparerProvider);
            var obj = new NonBuiltInEmptyClass();

            bool result = comparer.AreEqual(obj, obj);

            Assert.IsTrue(result, "The objects should be equal as they are the actual same one.");
        }

        [Test]
        public void CompareSimpleObjectsThatShouldMatch()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInClass), _objectComparerProvider);
            var object1 = new NonBuiltInClass { TheValue = "Value1" };
            var object2 = new NonBuiltInClass { TheValue = "Value1" };

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result, "Object should match.");
        }

        [Test]
        public void CompareSimpleObjectsThatShouldNotMatch()
        {
            var comparer = new ObjectComparer(typeof(NonBuiltInClass), _objectComparerProvider);
            var object1 = new NonBuiltInClass { TheValue = "Value1" };
            var object2 = new NonBuiltInClass { TheValue = "Value2" };

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result, "Object should not match.");
        }

        [Test]
        public void CompareComplexObjectsThatShouldMatch()
        {
            var comparer = new ObjectComparer(typeof(ComplexNonBuiltInClass), _objectComparerProvider);
            var object1 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };
            var object2 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result, "Object should match.");
        }

        [Test]
        public void CompareComplexObjectsThatShouldNotMatch()
        {
            var comparer = new ObjectComparer(typeof(ComplexNonBuiltInClass), _objectComparerProvider);
            var object1 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };
            var object2 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value2"} };

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result, "Object should not match.");
        }

        [Test]
        public void CompareObjectsWithEnumerableProperties()
        {
            var comparer = new ObjectComparer(typeof(ComplexNonBuiltInWithEnumerablePropertyClass), _objectComparerProvider);
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result, "Objects should be equal.");
        }

        [Test]
        public void CompareObjectsWithUnevenEnumerableProperties()
        {
            var comparer = new ObjectComparer(typeof(ComplexNonBuiltInWithEnumerablePropertyClass), _objectComparerProvider);
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(3);

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result, "Objects should not be equal.");
        }

        [Test]
        public void CompareObjectsWithUnevenEnumerableProperties2()
        {
            var comparer = new ObjectComparer(typeof(ComplexNonBuiltInWithEnumerablePropertyClass), _objectComparerProvider);
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(3);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);

            bool result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result, "Objects should not be equal.");
        }

        private static ComplexNonBuiltInWithEnumerablePropertyClass CreateComplexNonBuiltInWithEnumerablePropertyClass(int i)
        {
            var object1 = new ComplexNonBuiltInWithEnumerablePropertyClass
            {
                StringValues = new[] { "Value1", "Value2" },
                ComplexValues =
                    Enumerable.Range(1, i)
                              .Select(x =>
                                      new ComplexNonBuiltInClass
                                      {
                                          TheValue = new NonBuiltInClass
                                          {
                                              TheValue = string.Format("DeepValue{0}", x)
                                          }
                                      })
                              .ToArray()
            };

            return object1;
        }
    }

    public class ComplexNonBuiltInWithEnumerablePropertyClass
    {
        public string[] StringValues { get; set; }

        public IList<ComplexNonBuiltInClass> ComplexValues { get; set; }
    }

    public class NonBuiltInEmptyClass
    {
    }

    public class NonBuiltInClass
    {
        public string TheValue { get; set; }
    }

    public class ComplexNonBuiltInClass
    {
        public NonBuiltInClass TheValue { get; set; }
    }
}