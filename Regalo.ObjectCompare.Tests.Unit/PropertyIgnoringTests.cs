using NUnit.Framework;

namespace Regalo.ObjectCompare.Tests.Unit
{
    [TestFixture]
    public class PropertyIgnoringTests
    {
        [Test]
        public void IgnoreSingleProperty()
        {
            var ignoreList = new PropertyComparisonIgnoreList();
            ignoreList.Add<SimpleObject, string>(x => x.StringProperty1);

            Assert.That(ignoreList.Contains(typeof(SimpleObject), "StringProperty1"), Is.True);
        }
    }

    public class SimpleObject
    {
        public string StringProperty1 { get; set; }
        public string StringProperty2 { get; set; }
    }
}