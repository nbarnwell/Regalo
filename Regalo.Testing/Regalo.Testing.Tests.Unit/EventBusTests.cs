using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Regalo.Core;

namespace Regalo.Testing.Tests.Unit
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public void GivenNoPreviousState_WhenPublishingEvents_ThenEventsShouldBeStored()
        {
            // Arrange
            var eventBus = new FakeEventBus();

            // Act
            

            // Assert

        }
    }
}