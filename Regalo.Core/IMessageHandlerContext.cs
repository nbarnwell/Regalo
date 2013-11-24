using System;

namespace Regalo.Core
{
    public interface IMessageHandlerContext<TEntity>
    {
        TEntity Get(Guid id);
        void SaveAndPublishEvents(TEntity entity);
    }
}