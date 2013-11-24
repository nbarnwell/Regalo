namespace Regalo.Testing
{
    public interface IScenarioAssert<TEntity, THandler, TCommand>
    {
        void Assert();
    }
}