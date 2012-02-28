namespace Regalo.Core
{
    public interface IRepository<T>
        where T : AggregateRoot, new()
    {
        T Get(string id);
        void Save(T item);
    }
}