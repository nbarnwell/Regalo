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
        private CommandHandlerA _commandHandlerA;
        private CommandHandlerB _commandHandlerB;
            
        [SetUp]
        public void SetUp()
        {
            _objectCommandHandler = new ObjectCommandHandler();
            _commandHandlerA = new CommandHandlerA();
            _commandHandlerB = new CommandHandlerB();
            Resolver.SetResolvers(type => null, LocateAllCommandHandlers);
        }

        private IEnumerable<object> LocateAllCommandHandlers(Type type)
        {
            return new object[] { _objectCommandHandler, _commandHandlerA, _commandHandlerB }
                .Where(x => type.IsAssignableFrom(x.GetType()));
        }

        [TearDown]
        public void TearDown()
        {
            Resolver.ClearResolvers();
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
            Resolver.ClearResolvers();
            Resolver.SetResolvers(
                type => null,
                type =>
                {
                    result.Add(type);
                    return LocateAllCommandHandlers(type);
                });

            var processor = new CommandProcessor(new NullLogger());

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

            var processor = new CommandProcessor(new NullLogger());

            processor.Process(new SimpleCommand());

            _objectCommandHandler.Messages.ToList().ForEach(Console.WriteLine);

            CollectionAssert.AreEqual(expected, _objectCommandHandler.Messages);
        }

        [Test]
        public void GivenAMessageHandledMultipleHandlers_WhenAskedToProcess_ShouldInvokeAllCommandHandlersInCorrectSequence()
        {
            var processor = new CommandProcessor(new NullLogger());

            processor.Process(new CommandHandledByMultipleHandlers());

            CollectionAssert.AreEqual(new [] { typeof(object) }, _objectCommandHandler.Messages);
            CollectionAssert.AreEqual(new [] { typeof(CommandHandledByMultipleHandlers) }, _commandHandlerA.Messages);
            CollectionAssert.AreEqual(new [] { typeof(CommandHandledByMultipleHandlers) }, _commandHandlerB.Messages);
        }
    }

    public class CommandHandledByMultipleHandlers
    {
    }

    public class CommandHandlerA : ICommandHandler<CommandHandledByMultipleHandlers>
    {
        public readonly IList<Type> Messages = new List<Type>();

        public void Handle(CommandHandledByMultipleHandlers command)
        {
            Messages.Add(typeof(CommandHandledByMultipleHandlers));
        }
    }

    public class CommandHandlerB : ICommandHandler<CommandHandledByMultipleHandlers>
    {
        public readonly IList<Type> Messages = new List<Type>();

        public void Handle(CommandHandledByMultipleHandlers command)
        {
            Messages.Add(typeof(CommandHandledByMultipleHandlers));
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