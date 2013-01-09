using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Regalo.Core.Tests.Unit
{
    [TestFixture]
    public class CommandProcessorTests
    {
        private ObjectCommandHandler _objectCommandHandler;
            
        [SetUp]
        public void SetUp()
        {
            _objectCommandHandler = new ObjectCommandHandler();
            Resolver.SetResolver(LocateCommandHandler);
        }

        private object LocateCommandHandler(Type type)
        {
            if (type.IsAssignableFrom(_objectCommandHandler.GetType()))
            {
                return _objectCommandHandler;
            }

            throw new NotSupportedException(string.Format("No handler for type {0} registered.", type));
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolver();
        }

        [Test]
        public void GivenAMessage_WhenAskedToProcess_ShouldTryToFindHandlersForMessageTypeHierarchy()
        {
            var expected = new[]
            {
                typeof(ICommandHandler<object>),
                typeof(ICommandHandler<SimpleCommandBase>),
                typeof(ICommandHandler<SimpleCommand>),
            };

            var result = new List<Type>();
            Resolver.ClearResolver();
            Resolver.SetResolver(type =>
            {
                result.Add(type);
                return LocateCommandHandler(type);
            });

            var processor = new CommandProcessor();

            processor.Process(new SimpleCommand());

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void GivenAMessage_WhenAskedToProcess_ShouldInvokeCommandHandlersInCorrectSequence()
        {
            var expected = new[]
            {
                typeof(object),
                typeof(SimpleCommandBase),
                typeof(SimpleCommand),
            };

            var processor = new CommandProcessor();

            processor.Process(new SimpleCommand());

            _objectCommandHandler.Messages.ToList().ForEach(Console.WriteLine);

            CollectionAssert.AreEqual(expected, _objectCommandHandler.Messages);
        }
    }

    public class SimpleCommand : SimpleCommandBase
    {
    }

    public class SimpleCommandBase // Remember this inherits from object...
    {
    }

    public class ObjectCommandHandler 
        : ICommandHandler<object>,
        ICommandHandler<SimpleCommandBase>,
        ICommandHandler<SimpleCommand>
    {
        public readonly IList<Type> Messages = new List<Type>(); 

        public void Handle(object command)
        {
            Messages.Add(typeof(object));
        }

        void ICommandHandler<SimpleCommandBase>.Handle(SimpleCommandBase command)
        {
            Messages.Add(typeof(SimpleCommandBase));
        }

        public void Handle(SimpleCommand command)
        {
            Messages.Add(typeof(SimpleCommand));
        }
    }
}