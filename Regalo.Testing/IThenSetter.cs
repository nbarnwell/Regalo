using System;

namespace Regalo.Testing
{
    public interface IThenSetter<TEntity, THandler, TCommand>
    {
        IScenarioAssert<TEntity, THandler, TCommand> Then(Func<TEntity, TCommand, object[]> func);
        IScenarioExceptionAssert<TException, TEntity, THandler, TCommand> Throws<TException>() where TException : Exception;
    }
}