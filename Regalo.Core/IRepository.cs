using System;

namespace Regalo.Core
{
    public interface IRepository<T>
        where T : AggregateRoot, new()
    {
        T Get(Guid id);
        void Save(T item);
    }
}