namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class TransactionPlaced : Event
    {
        public string AggregateId { get; private set; }
        public decimal Amount { get; private set; }
        public string[] Categories { get; private set; }

        public TransactionPlaced(string accountId, decimal amount, string[] categories)
        {
            AggregateId = accountId;
            Amount = amount;
            Categories = categories;
        }

        public bool Equals(TransactionPlaced other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.AggregateId, AggregateId) && other.Amount == Amount && Equals(other.Categories, Categories);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(TransactionPlaced)) return false;
            return Equals((TransactionPlaced)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (AggregateId != null ? AggregateId.GetHashCode() : 0);
                result = (result*397) ^ Amount.GetHashCode();
                result = (result*397) ^ (Categories != null ? Categories.GetHashCode() : 0);
                return result;
            }
        }
    }
}