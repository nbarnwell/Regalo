namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class ItemsAddedToOrder : Event
    {
        public ItemsAddedToOrder(string orderId, string sku, uint quantity)
        {
            AggregateId = orderId;
            Sku = sku;
            Quantity = quantity;
        }

        public string AggregateId { get; private set; }

        public string Sku { get; private set; }

        public uint Quantity { get; private set; }
    }
}