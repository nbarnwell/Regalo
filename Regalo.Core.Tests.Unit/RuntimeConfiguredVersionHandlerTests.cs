using System;
using NUnit.Framework;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class RuntimeConfiguredVersionHandlerTests
    {
        [Test]
        public void GivenAnyEvent_WhenQueryingVersion_ShouldReturnCorrectVersion()
        {
            // Arrange
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration(typeof(UserRegistered), e => ((UserRegistered)e).Version, null);
            var evt = new UserRegistered(Guid.NewGuid());

            // Act
            Guid version = versionHandler.GetVersion(evt);

            // Assert
            Assert.AreEqual(evt.Version, version);
        }

        [Test]
        public void GivenEventList_WhenQueryingCurrentVersion_ShouldReturnVersionOfLastEvent()
        {
            // Arrange
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration(typeof(UserRegistered), e => ((UserRegistered)e).Version, null);
            var userId = Guid.NewGuid();
            var lastVersion = Guid.NewGuid();
            var events = new[]
            {
                new UserRegistered(userId) { Version = Guid.NewGuid() },
                new UserRegistered(userId) { Version = Guid.NewGuid() },
                new UserRegistered(userId) { Version = Guid.NewGuid() },
                new UserRegistered(userId) { Version = Guid.NewGuid() },
                new UserRegistered(userId) { Version = Guid.NewGuid() },
                new UserRegistered(userId) { Version = lastVersion }
            };

            // Act
            Guid version = versionHandler.GetCurrentVersion(events);

            // Assert
            Assert.AreEqual(lastVersion, version);
        }
    }
}