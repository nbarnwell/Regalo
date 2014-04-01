using System;
using Moq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Regalo.Core;
using Regalo.RavenDB.Tests.Unit.DomainModel.Customers;
using Regalo.Testing;

namespace Regalo.RavenDB.Tests.Unit
{
    [TestFixture]
    public class DelayedWriteEventStoreTests
    {
        private IDocumentStore _documentStore;
        private Mock<IVersionHandler> _versionHandlerMock;
        
        [SetUp]
        public void SetUp()
        {
            _documentStore = new EmbeddableDocumentStore { RunInMemory = true };
            //_documentStore = new DocumentStore
            //{
            //    Url = "http://localhost:8080/",
            //    DefaultDatabase = "Regalo.RavenDB.Tests.UnitPersistenceTests"
            //};
            _documentStore.Initialize();

            _versionHandlerMock = new Mock<IVersionHandler>();
            _versionHandlerMock.Setup(x => x.GetVersion(It.IsAny<Event>())).Returns<Event>(x => x.Version);
            _versionHandlerMock.Setup(x => x.SetParentVersion(It.IsAny<Event>(), It.IsAny<Guid?>())).Callback<object, Guid?>((x, v) => ((Event)x).ParentVersion = v);
            Resolver.SetResolvers(type =>
            {
                if (type == typeof(IVersionHandler)) return _versionHandlerMock.Object;
                if (type == typeof(ILogger)) return new NullLogger();
                throw new InvalidOperationException(string.Format("No type of {0} registered.", type));
            },
            type => null);
        }

        [TearDown]
        public void TearDown()
        {
            Conventions.SetFindAggregateTypeForEventType(null);

            Resolver.ClearResolvers();

            _documentStore.Dispose();
            _documentStore = null;
        }

        [Test]
        public void Disposing_a_delayedwriteeventstore_with_pending_changes_should_throw_exception()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var eventStore = new DelayedWriteRavenEventStore(_documentStore, _versionHandlerMock.Object))
                {
                    var customerId = Guid.NewGuid();

                    var storedEvents = new object[]
                    {
                        new CustomerSignedUp(customerId),
                        new SubscribedToNewsletter("latest"),
                        new SubscribedToNewsletter("top")
                    };

                    eventStore.Add(customerId, storedEvents);
                }
            });
        }
    }
}