using System;

namespace Regalo.Core
{
    public interface IRepository<T>
        where T : AggregateRoot, new()
    {
        T Get(Guid id);
        T Get(Guid id, Guid version);
        void Save(T item);
    }
}