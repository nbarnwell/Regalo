using System;
using NUnit.Framework;
using Regalo.Core.Tests.DomainModel.Users;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class RuntimeConfiguredVersionHandlerTests
    {
        [Test]
        public void GivenAnyEvent_WhenQueryingVersion_ThenShouldReturnCorrectVersion()
        {
            // Arrange
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration<UserRegistered>(e => e.Version, null);
            var evt = new UserRegistered(Guid.NewGuid());

            // Act
            Guid version = versionHandler.GetVersion(evt);

            // Assert
            Assert.AreEqual(evt.Version, version);
        }

        [Test]
        public void GivenNewEvent_WhenSettingParentVersion_ThenShouldSetParentVersion()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var userRegisteredEvent = new UserRegistered(userId);
            var userChangedPasswordEvent = new UserChangedPassword("newpassword");
            
            var versionHandler = new RuntimeConfiguredVersionHandler();
            versionHandler.AddConfiguration<UserChangedPassword>(null, (e, v) => e.ParentVersion = v);

            // Act
            versionHandler.SetParentVersion(userChangedPasswordEvent, userRegisteredEvent.Version);

            // Assert
            Assert.AreEqual(userRegisteredEvent.Version, userChangedPasswordEvent.ParentVersion);
        }
    }
}