using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class TypeInspectorTests
    {
        [Test]
        [TestCase(null,                                                       new Type[] { })]
        [TestCase(typeof(object),                                             new[] { typeof(object)  })]
        [TestCase(typeof(ClassWithNoSpecificBase),                            new[] { typeof(object), typeof(ClassWithNoSpecificBase)  })]
        [TestCase(typeof(ClassNotDirectlyInheritingObject),                   new[] { typeof(object), typeof(ClassWithNoSpecificBase), typeof(ClassNotDirectlyInheritingObject)                 })]
        [TestCase(typeof(ClassWithNoSpecificBaseThatImplementsAnInterface),   new[] { typeof(object), typeof(IAmAnInterface),          typeof(ClassWithNoSpecificBaseThatImplementsAnInterface) })]
        [TestCase(typeof(VerySpecificSuperClass),                             new[] { typeof(object), typeof(ClassWithNoSpecificBase), typeof(ClassNotDirectlyInheritingObject),                typeof(VerySpecificSuperClass)                             })]
        [TestCase(typeof(ClassWithNoSpecificBaseThatImplementsTwoInterfaces), new[] { typeof(object), typeof(IAmAnInterface),          typeof(IAmAnInterfaceToo),                               typeof(ClassWithNoSpecificBaseThatImplementsTwoInterfaces) })]
        public void GivenAType_WhenSearchingForTypeHierarchy_ShouldReturnBaseTypesAndInputType(Type input, IEnumerable<Type> expectedResult)
        {
            var inspector = new TypeInspector();

            var result = inspector.GetTypeHierarchy(input);

            CollectionAssert.AreEqual(expectedResult, result);
        }
    }

    public class ClassWithNoSpecificBaseThatImplementsTwoInterfaces : IAmAnInterface, IAmAnInterfaceToo
    { }

    public class ClassWithNoSpecificBaseThatImplementsAnInterface : IAmAnInterface
    { }

    public interface IAmAnInterface
    { }

    public interface IAmAnInterfaceToo
    { }

    public class VerySpecificSuperClass : ClassNotDirectlyInheritingObject
    { }

    public class ClassNotDirectlyInheritingObject : ClassWithNoSpecificBase
    { }

    public class ClassWithNoSpecificBase
    { }
}