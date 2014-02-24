using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Regalo.Core;

namespace Regalo.Testing
{
    public class ScenarioExceptionAssert<TException, TEntity, THandler, TCommand> 
        : ScenarioAssertBase<THandler, TCommand>
        , IScenarioExceptionAssert<TException, TEntity, THandler, TCommand> 
        where TEntity : AggregateRoot, new() 
        where TException : Exception
    {
        public ScenarioExceptionAssert(THandler handler, TCommand command) 
            : base(handler, command)
        {
        }

        public void Assert()
        {
            Exception exception = null;
            try
            {
                InvokeHandler();
            }
            catch (Exception ex)
            {
                exception = ex.GetBaseException();
            }

            NUnit.Framework.Assert.That(
                (object)exception,
                (IResolveConstraint)new ExceptionTypeConstraint(typeof(TException)),
                "Expected exception was not thrown.",
                null);
        }
    }
}