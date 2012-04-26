using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Regalo.Core;

namespace Regalo.EventSourcing.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class PersistenceTests
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
        public void Save_GivenAggregateRoot_ShouldStoreEventsInEventStore()
        {
            // Arrange
            var customer = new Customer();

            // Act


            // Assert

        }
    }

    // ReSharper restore InconsistentNaming

    public class Customer : AggregateRoot
    {
        public void Signup()
        {
            Record(new CustomerSignedUp(Guid.NewGuid().ToString()));
        }

        private void Apply(CustomerSignedUp evt)
        {
            Id = evt.AggregateId;
        }
    }

    public class CustomerSignedUp : Event
    {
        public string AggregateId { get; set; }

        public CustomerSignedUp(string customerId)
        {
            AggregateId = customerId;
        }

        public bool Equals(CustomerSignedUp other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.AggregateId, AggregateId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CustomerSignedUp)) return false;
            return Equals((CustomerSignedUp)obj);
        }

        public override int GetHashCode()
        {
            return (AggregateId != null ? AggregateId.GetHashCode() : 0);
        }
    }
}