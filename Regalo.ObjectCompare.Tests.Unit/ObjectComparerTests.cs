using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Regalo.ObjectCompare.Tests.Unit
{
    [TestFixture]
    public class ObjectComparerTests
    {
        private IObjectComparer comparer;

        [SetUp]
        public void SetUp()
        {
            comparer = new ObjectComparer();
        }

        [TearDown]
        public void TearDown()
        {
            comparer = null;
        }

        [Test]
        public void CompareEmptyObjects()
        {
            var object1 = new NonBuiltInEmptyClass();
            var object2 = new NonBuiltInEmptyClass();

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "The objects are not equal, despite being empty.");
        }

        [Test]
        public void CompareNullWithAnObject()
        {
            var object1 = new NonBuiltInEmptyClass();

            var result = comparer.AreEqual(object1, null);

            Assert.IsFalse(result.AreEqual, "Object reference and null are not equal.");
        }

        [Test]
        public void CompareNullWithNull()
        {
            var result = comparer.AreEqual(null, null);

            Assert.IsTrue(result.AreEqual, "null reference and null are equal.");
        }

        [Test]
        public void CompareSameObjects()
        {
            var obj = new NonBuiltInEmptyClass();

            var result = comparer.AreEqual(obj, obj);

            Assert.IsTrue(result.AreEqual, "The objects should be equal as they are the actual same one.");
        }

        [Test]
        public void CompareSimpleObjectsThatShouldMatch()
        {
            var object1 = new NonBuiltInClass { TheValue = "Value1" };
            var object2 = new NonBuiltInClass { TheValue = "Value1" };

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "Object should match.");
        }

        [Test]
        public void CompareSimpleObjectsThatShouldNotMatch()
        {
            var object1 = new NonBuiltInClass { TheValue = "Value1" };
            var object2 = new NonBuiltInClass { TheValue = "Value2" };

            var result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result.AreEqual, "Object should not match.");
        }

        [Test]
        public void CompareComplexObjectsThatShouldMatch()
        {
            var object1 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };
            var object2 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "Object should match.");
        }

        [Test]
        public void CompareComplexObjectsThatShouldNotMatch()
        {
            var object1 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value1"} };
            var object2 = new ComplexNonBuiltInClass { TheValue = new NonBuiltInClass{TheValue = "Value2"} };

            var result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result.AreEqual, "Object should not match.");
        }

        [Test]
        public void CompareObjectsWithEnumerableProperties()
        {
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "Objects should be equal.");
        }

        [Test]
        public void CompareObjectsWithUnevenEnumerableProperties()
        {
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(3);

            var result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result.AreEqual, "Objects should not be equal.");
        }

        [Test]
        public void CompareObjectsWithUnevenEnumerableProperties2()
        {
            var object1 = CreateComplexNonBuiltInWithEnumerablePropertyClass(3);
            var object2 = CreateComplexNonBuiltInWithEnumerablePropertyClass(2);

            var result = comparer.AreEqual(object1, object2);

            Assert.IsFalse(result.AreEqual, "Objects should not be equal.");
        }

        [Test]
        public void IgnoreSpecificProperties()
        {
            comparer.Ignore<NonBuiltInClass, string>(x => x.TheValue);
            
            var object1 = new NonBuiltInClass();
            var object2 = new NonBuiltInClass();
            object1.TheValue = "Something";
            object2.TheValue = "SomethingElse";

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "Objects should be equal despite ignored property being different.");
        }

        [Test]
        public void ComparingObjectsWithCircularReferencesShouldNotFail()
        {
            var object1 = new NonBuiltInClassAllowingCircularReference();
            object1.Inner = object1;

            var object2 = new NonBuiltInClassAllowingCircularReference();
            object2.Inner = object2;

            var result = comparer.AreEqual(object1, object2);

            Assert.IsTrue(result.AreEqual, "Objects should be equal and result returned despite circular reference.");
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

    public class NonBuiltInClassAllowingCircularReference
    {
        public NonBuiltInClassAllowingCircularReference Inner { get; set; }
    }
}