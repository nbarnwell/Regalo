using System;
using NUnit.Framework;

namespace Regalo.Core.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class MessageDsiplayFormattingTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ToString_GivenEventWithSingleProperty_ShouldReturnStandardFormattedString()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var evt = new UserChangedPassword(userId, "newpassword");
            var expected = string.Format("UserChangedPassword event for aggregate {0} with NewPassword: \"{1}\"", userId, evt.NewPassword);

            // Act
            string asString = evt.ToString();

            // Assert
            Assert.AreEqual(expected, asString, "Event was not correctly formatted to string.");
        }

        [Test]
        public void ToString_GivenEventWithMultipleProperties_ShouldReturnStandardFormattedString()
        {
            // Arrange
            string orderId = Guid.NewGuid().ToString();
            var evt = new ItemsAddedToOrder(orderId, "sku", 12);
            var expected = string.Format("ItemsAddedToOrder event for aggregate {0} with Sku: \"{1}\", Quantity: {2}", orderId, evt.Sku, evt.Quantity);

            // Act
            string asString = evt.ToString();

            // Assert
            Assert.AreEqual(expected, asString, "Event was not correctly formatted to string.");
        }

        [Test]
        public void ToString_GivenEventWithEnumerableProperty_ShouldReturnStandardFormattedString()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var evt = new TransactionPlaced(accountId, (decimal)123.12, new[] { "Groceries", "Petrol" });
            var expected = string.Format("TransactionPlaced event for aggregate {0} with Amount: {1}, Categories: {2}", accountId, evt.Amount, "String[]");

            // Act
            string asString = evt.ToString();

            // Assert
            Assert.AreEqual(expected, asString, "Event was not correctly formatted to string.");
        }

        [Test]
        public void ToString_GivenEventWithNullProperty_ShouldReturnStandardFormattedString()
        {
            // Arrange
            string orderId = Guid.NewGuid().ToString();
            var evt = new ItemsAddedToOrder(orderId, null, 12);
            var expected = string.Format("ItemsAddedToOrder event for aggregate {0} with Sku: {1}, Quantity: {2}", orderId, "<null>", evt.Quantity);

            // Act
            string asString = evt.ToString();

            // Assert
            Assert.AreEqual(expected, asString, "Event was not correctly formatted to string.");
        }
    }

    // ReSharper restore InconsistentNaming
}