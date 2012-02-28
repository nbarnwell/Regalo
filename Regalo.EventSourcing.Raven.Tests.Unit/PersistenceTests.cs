using System.Collections.Generic;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Regalo.Core;

namespace Regalo.EventSourcing.Raven.Tests.Unit
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void Loading_GivenEmptyStore_ShouldReturnNull()
        {
            // Arrange
            IDocumentStore documentStore = new EmbeddableDocumentStore { RunInMemory = true };
            documentStore.Initialize();
            IRepository<Customer> repository = new RavenRepository<Customer>(documentStore);

            // Act
            Customer customer = repository.Get("customer1");

            // Assert
            Assert.Null(customer);
        }
    }

    public class Customer : AggregateRoot
    {
    }
}