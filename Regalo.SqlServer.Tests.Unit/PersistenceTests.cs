using System;
using NUnit.Framework;
using Regalo.Core;

namespace Regalo.SqlServer.Tests.Unit
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void Loading_GivenEmptyStore_ShouldReturnNull()
        {
            // Arrange
            IRepository<Customer> repository = new SqlServerRepository<Customer>("");

            // Act
            Customer customer = repository.Get("customer1");

            // Assert
            Assert.Null(customer);
        }

        [Test]
        public void Saving_GivenNewAggregate_ShouldAllowReloading()
        {
            // Arrange
            IRepository<Customer> repository = new SqlServerRepository<Customer>("");

            // Act
            var customer = new Customer();
            string id = customer.Id;
            repository.Save(customer);
            customer = repository.Get(id);

            // Assert
            Assert.NotNull(customer);
            Assert.AreEqual(id, customer.Id);
        }

        public class Customer : AggregateRoot
        {
            public Customer()
            {
                Record(new CustomerCreated(Guid.NewGuid().ToString()));
            }

            private void Apply(CustomerCreated evt)
            {
                Id = evt.AggregateId;
            }
        }

        public class CustomerCreated : Event
        {
            public CustomerCreated(string id)
                : base(id)
            {

            }
        }
    }
}