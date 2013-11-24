using System;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class SalesOrderCreated : Event
    {
        public Guid AggregateId { get; private set; }

        public SalesOrderCreated(Guid id)
        {
            AggregateId = id;
        }
    }
}