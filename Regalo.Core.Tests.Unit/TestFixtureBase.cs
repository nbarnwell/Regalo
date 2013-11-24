using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using Regalo.Core.Tests.DomainModel.Users;
using Regalo.Testing;

namespace Regalo.Core.Tests.Unit
{
    public abstract class TestFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            var _versionHandler = new RuntimeConfiguredVersionHandler();
            _versionHandler.AddConfiguration<UserChangedPassword>(e => e.Version, (e, v) => e.ParentVersion = v);
            _versionHandler.AddConfiguration<UserRegistered>(e => e.Version, (e, v) => e.ParentVersion = v);

            var _nullLogger = new NullLogger();

            Resolver.SetResolvers(
                type =>
                {
                    if (type == typeof(IVersionHandler))
                    {
                        return _versionHandler;
                    }

                    if (type == typeof(ILogger))
                    {
                        return _nullLogger;
                    }

                    throw new NotSupportedException(string.Format("TestFixtureBase::SetUp - Nothing registered for {0}", type));
                },
                type => null);
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolvers();
        }

        protected void CollectionAssertAreJsonEqual(IEnumerable<object> expected, IEnumerable<object> actual)
        {
            var expectedJson = GetJsonList(expected);
            var actualJson   = GetJsonList(actual);

            CollectionAssert.AreEqual(expectedJson, actualJson);
        }

        protected void AssertAreJsonEqual(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
            var actualJson   = JsonConvert.SerializeObject(actual, Formatting.Indented);

            Assert.AreEqual(expectedJson, actualJson);
        }

        private IEnumerable<string> GetJsonList(IEnumerable<object> list)
        {
            return list.Select(x => JsonConvert.SerializeObject(x, Formatting.Indented))
                .Select(FixGuids)
                .ToArray();
        }

        private string FixGuids(string json)
        {
            var result = json;

            result = Regex.Replace(
                    result,
                    @"""Id""\s*:\s*""(?i:[a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12})""",
                    @"""Id"" : ""00000000-0000-0000-0000-000000000000""");

            result = Regex.Replace(
                    result,
                    @"""Version""\s*:\s*""(?i:[a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12})""",
                    @"""Version"" : ""00000000-0000-0000-0000-000000000000""");

            result =
                Regex.Replace(
                    result,
                    @"""ParentVersion""\s*:\s*""(?i:[a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12})""",
                    @"""ParentVersion"" : ""00000000-0000-0000-0000-000000000000""");

            return result;
        }
    }
}