using System;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class SalesOrder : AggregateRoot
    {
        public void Create(Guid id)
        {
            Record(new SalesOrderCreated(id));
        }

        public void AddLine(string sku, uint quantity)
        {
            Record(new ItemsAddedToOrder(Id, sku, quantity));
        }

        public void PlaceOrder()
        {
            Record(new SalesOrderPlaced(Id));
        }

        private void Apply(SalesOrderCreated evt)
        {
            Id = evt.AggregateId;
        }
    }
}