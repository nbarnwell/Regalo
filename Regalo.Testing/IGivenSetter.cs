namespace Regalo.Testing
{
    public interface IGivenSetter<TEntity, THandler>
    {
        IWhenSetter<TEntity, THandler> Given(ITestDataBuilder<TEntity> testDataBuilder);
    }
}