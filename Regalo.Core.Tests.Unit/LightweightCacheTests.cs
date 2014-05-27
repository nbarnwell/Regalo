using System;
using NUnit.Framework;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class LightweightCacheTests
    {
        [Test]
        public void GivenEmptyCache_WhenAskingForItem_ThenShouldLoadItem()
        {
            var eventType = typeof(SimpleEvent);
            var loaderUsageCount = 0;

            var cache = new LightweightCache<RuntimeTypeHandle, bool>(
                typeHandle =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType.TypeHandle);

            Assert.That(result, Is.False);
            Assert.That(loaderUsageCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEmptyCache_WhenAskingForItemTwice_ThenShouldLoadItemOnlyOnce()
        {
            var eventType = typeof(SimpleEvent);
            var loaderUsageCount = 0;

            var cache = new LightweightCache<RuntimeTypeHandle, bool>(
                typeHandle =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType.TypeHandle);
            Assert.That(result, Is.False);
            
            result = cache.GetValue(eventType.TypeHandle);
            Assert.That(result, Is.False);

            Assert.That(loaderUsageCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEmptyCache_WhenAskingForItemTwiceForSomethingOtherThanTheKey_ThenShouldLoadItemOnlyOnce()
        {
            var eventType = typeof(SimpleEvent);
            var loaderUsageCount = 0;

            var cache = new LightweightCacheForRuntimeKey<Type, RuntimeTypeHandle, bool>(
                type =>
                {
                    return type.TypeHandle;
                },
                typeHandle =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType.TypeHandle);
            Assert.That(result, Is.False);
            
            result = cache.GetValue(eventType.TypeHandle);
            Assert.That(result, Is.False);

            Assert.That(loaderUsageCount, Is.EqualTo(1));
        }
    }
}