namespace Regalo.Testing
{
    public interface IHandlerSetter<TEntity>
    {
        IGivenSetter<TEntity, THandler> HandledBy<THandler>(THandler handler);
    }
}