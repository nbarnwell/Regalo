namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class TransactionPlaced : Event
    {
        public TransactionPlaced(string accountId, decimal amount, string[] categories)
        {
            AggregateId = accountId;
            Amount = amount;
            Categories = categories;
        }

        public string AggregateId { get; private set; }

        public decimal Amount { get; private set; }

        public string[] Categories { get; private set; }
    }
}