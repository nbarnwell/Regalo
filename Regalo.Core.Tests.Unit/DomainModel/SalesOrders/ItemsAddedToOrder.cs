namespace Regalo.Core.Tests.Unit.DomainModel.SalesOrders
{
    public class ItemsAddedToOrder : Event
    {
        public string AggregateId { get; private set; }
        public string Sku { get; private set; }
        public uint Quantity { get; private set; }

        public ItemsAddedToOrder(string orderId, string sku, uint quantity)
        {
            AggregateId = orderId;
            Sku = sku;
            Quantity = quantity;
        }

        public bool Equals(ItemsAddedToOrder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.AggregateId, AggregateId) && Equals(other.Sku, Sku) && other.Quantity == Quantity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ItemsAddedToOrder)) return false;
            return Equals((ItemsAddedToOrder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (AggregateId != null ? AggregateId.GetHashCode() : 0);
                result = (result*397) ^ (Sku != null ? Sku.GetHashCode() : 0);
                result = (result*397) ^ Quantity.GetHashCode();
                return result;
            }
        }
    }
}