using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    public abstract class TestFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration<UserChangedPassword>(e => e.Version, (e, v) => e.ParentVersion = v);
            versionHandler.AddConfiguration<UserRegistered>(e => e.Version, (e, v) => e.ParentVersion = v);

            Resolver.SetResolver(
                t =>
                {
                    if (t == typeof(IVersionHandler))
                    {
                        return versionHandler;
                    }

                    throw new NotSupportedException(string.Format("Nothing registered in SetUp for {0}", t));
                });
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolver();
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
                .Select(FixVersionGuids)
                .ToArray();
        }

        private string FixVersionGuids(string json)
        {
            var result = json;

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