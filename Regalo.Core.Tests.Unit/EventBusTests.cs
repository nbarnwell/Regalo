using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Regalo.Core;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class EventBusTests
    {
        private ObjectEventHandler _objectEventHandler;

        [SetUp]
        public void SetUp()
        {
            _objectEventHandler = new ObjectEventHandler();
            Resolver.SetResolver(LocateEventHandler);
        }

        private object LocateEventHandler(Type type)
        {
            if (type.IsAssignableFrom(_objectEventHandler.GetType()))
            {
                return _objectEventHandler;
            }

            throw new NotSupportedException(string.Format("No handler for type {0} registered.", type));
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolver();
        }

        [Test]
        public void GivenAMessage_WhenAskedToPublish_ShouldTryToFindHandlersForMessageTypeHierarchy()
        {
            var expected = new[]
            {
                typeof(IEventHandler<object>),
                typeof(IEventHandler<SimpleEventBase>),
                typeof(IEventHandler<SimpleEvent>),
            };

            var result = new List<Type>();
            Resolver.ClearResolver();
            Resolver.SetResolver(type =>
            {
                result.Add(type);
                return LocateEventHandler(type);
            });

            var processor = new EventBus();

            processor.Publish(new SimpleEvent());

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void GivenAMessage_WhenAskedToProcess_ShouldInvokeHandlersInCorrectSequence()
        {
            var expected = new[]
            {
                typeof(object),
                typeof(SimpleEventBase),
                typeof(SimpleEvent),
            };

            var processor = new EventBus();

            processor.Publish(new SimpleEvent());

            _objectEventHandler.Messages.ToList().ForEach(Console.WriteLine);

            CollectionAssert.AreEqual(expected, _objectEventHandler.Messages);
        }
    }
}

public class SimpleEvent : SimpleEventBase
{
}

public class SimpleEventBase // Remember this inherits from object...
{
}

public class ObjectEventHandler
    : IEventHandler<object>,
    IEventHandler<SimpleEventBase>,
    IEventHandler<SimpleEvent>
{
    public readonly IList<Type> Messages = new List<Type>();

    public void Handle(object evt)
    {
        Messages.Add(typeof(object));
    }

    void IEventHandler<SimpleEventBase>.Handle(SimpleEventBase command)
    {
        Messages.Add(typeof(SimpleEventBase));
    }

    public void Handle(SimpleEvent command)
    {
        Messages.Add(typeof(SimpleEvent));
    }
}
