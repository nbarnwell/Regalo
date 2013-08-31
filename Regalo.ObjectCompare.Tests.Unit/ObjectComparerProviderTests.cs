using NUnit.Framework;

namespace Regalo.ObjectCompare.Tests.Unit
{
    [TestFixture]
    public class ObjectComparerProviderTests
    {
        [Test]
        public void CreateComparerReturnsComparer()
        {
            var factory = new ObjectComparerProvider();

            var comparer = factory.ComparerFor(typeof(TestClass));
            
            Assert.NotNull(comparer);
        }

        [Test]
        public void CreateComparerAlwaysReturnsSameComparerForType()
        {
            var factory = new ObjectComparerProvider();

            var comparer1 = factory.ComparerFor(typeof(TestClass));
            var comparer2 = factory.ComparerFor(typeof(TestClass));
            
            Assert.AreSame(comparer1, comparer2);
        }
    }

    public class TestClass
    {
    }
}