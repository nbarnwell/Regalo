using System;
using NUnit.Framework;
using Regalo.Core;
using Regalo.Core.Tests.DomainModel.SalesOrders;

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

        [Test]
        public void IgnoreMultipleProperties()
        {
            var ignoreList = new PropertyComparisonIgnoreList();
            ignoreList.Add<SimpleObject, string>(x => x.StringProperty1);
            ignoreList.Add<SimpleObject, string>(x => x.StringProperty2);

            Assert.That(ignoreList.Contains(typeof(SimpleObject), "StringProperty1"), Is.True);
            Assert.That(ignoreList.Contains(typeof(SimpleObject), "StringProperty2"), Is.True);
        }

        [Test]
        public void IgnoreSamePropertyThroughoutTypeHierarchyParents()
        {
            var ignoreList = new PropertyComparisonIgnoreList();
            ignoreList.Add<SalesOrderCreated, Guid?>(x => x.ParentVersion);

            Assert.That(ignoreList.Contains(typeof(SalesOrderCreated), "ParentVersion"), Is.True);
            Assert.That(ignoreList.Contains(typeof(Event), "ParentVersion"), Is.True);
        }

        [Test]
        public void IgnoreSamePropertyThroughoutTypeHierarchyChildren()
        {
            var ignoreList = new PropertyComparisonIgnoreList();
            ignoreList.Add<Event, Guid?>(x => x.ParentVersion);

            Assert.That(ignoreList.Contains(typeof(SalesOrderCreated), "ParentVersion"), Is.True);
            Assert.That(ignoreList.Contains(typeof(Event), "ParentVersion"), Is.True);
        }
    }

    public class SimpleObject
    {
        public string StringProperty1 { get; set; }
        public string StringProperty2 { get; set; }
    }

    public class InheritsSimpleObject : SimpleObject
    {
    }
}