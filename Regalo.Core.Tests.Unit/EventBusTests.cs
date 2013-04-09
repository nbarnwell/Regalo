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
        private EventHandlerA _eventHandlerA;
        private EventHandlerB _eventHandlerB;

        [SetUp]
        public void SetUp()
        {
            _objectEventHandler = new ObjectEventHandler();
            _eventHandlerA = new EventHandlerA();
            _eventHandlerB = new EventHandlerB();
            Resolver.SetResolvers(type => null, LocateAllEventHandlers);
        }

        private IEnumerable<object> LocateAllEventHandlers(Type type)
        {
            return new object[] { _objectEventHandler, _eventHandlerA, _eventHandlerB }
                .Where(x => type.IsAssignableFrom(x.GetType()));
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolvers();
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
            Resolver.ClearResolvers();
            Resolver.SetResolvers(
                type => null,
                type =>
                {
                    result.Add(type);
                    return LocateAllEventHandlers(type);
                });

            var processor = new EventBus(new NullLogger());

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

            var processor = new EventBus(new NullLogger());

            processor.Publish(new SimpleEvent());

            _objectEventHandler.Messages.ToList().ForEach(Console.WriteLine);

            CollectionAssert.AreEqual(expected, _objectEventHandler.Messages);
        }

        [Test]
        public void GivenAMessageHandledMultipleHandlers_WhenAskedToPublish_ShouldInvokeAllCommandHandlersInCorrectSequence()
        {
            var processor = new EventBus(new NullLogger());

            processor.Publish(new EventHandledByMultipleHandlers());

            CollectionAssert.AreEqual(new[] { typeof(object) }, _objectEventHandler.Messages);
            CollectionAssert.AreEqual(new[] { typeof(EventHandledByMultipleHandlers) }, _eventHandlerA.Messages);
            CollectionAssert.AreEqual(new[] { typeof(EventHandledByMultipleHandlers) }, _eventHandlerB.Messages);
        }
    }
}

public class EventHandledByMultipleHandlers
{
}

public class EventHandlerA : IEventHandler<EventHandledByMultipleHandlers>
{
    public readonly IList<Type> Messages = new List<Type>();

    public void Handle(EventHandledByMultipleHandlers evt)
    {
        Messages.Add(typeof(EventHandledByMultipleHandlers));
    }
}

public class EventHandlerB : IEventHandler<EventHandledByMultipleHandlers>
{
    public readonly IList<Type> Messages = new List<Type>();

    public void Handle(EventHandledByMultipleHandlers evt)
    {
        Messages.Add(typeof(EventHandledByMultipleHandlers));
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
