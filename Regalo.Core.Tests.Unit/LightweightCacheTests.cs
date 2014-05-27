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

            var cache = new LightweightCacheForRuntimeKey<Type, RuntimeTypeHandle, bool>(
                type =>
                {
                    return type.TypeHandle;
                },
                (type, typeHandle) =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType);

            Assert.That(result, Is.False);
            Assert.That(loaderUsageCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEmptyCache_WhenAskingForItemTwice_ThenShouldLoadItemOnlyOnce()
        {
            var eventType = typeof(SimpleEvent);
            var loaderUsageCount = 0;

            var cache = new LightweightCacheForRuntimeKey<Type, RuntimeTypeHandle, bool>(
                type =>
                {
                    return type.TypeHandle;
                },
                (type, typeHandle) =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType);
            Assert.That(result, Is.False);
            
            result = cache.GetValue(eventType);
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
                (type, typeHandle) =>
                {
                    loaderUsageCount++;
                    return false;
                });

            var result = cache.GetValue(eventType);
            Assert.That(result, Is.False);
            
            result = cache.GetValue(eventType);
            Assert.That(result, Is.False);

            Assert.That(loaderUsageCount, Is.EqualTo(1));
        }
    }
}