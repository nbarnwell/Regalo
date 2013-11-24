using System;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class ItemsAddedToOrder : Event
    {
        public ItemsAddedToOrder(Guid orderId, string sku, uint quantity)
        {
            AggregateId = orderId;
            Sku = sku;
            Quantity = quantity;
        }

        public Guid AggregateId { get; private set; }

        public string Sku { get; private set; }

        public uint Quantity { get; private set; }
    }
}