using System;
using Regalo.Core.Tests.DomainModel.SalesOrders;

namespace Regalo.Testing.Tests.Unit
{
    public class SalesOrderTestDataBuilder : AggregateRootTestDataBuilderBase<SalesOrder>
    {
        private SalesOrder _order;

        private SalesOrderTestDataBuilder(SalesOrder order)
        {
            _order = order;
            CurrentDescription = "New order";
        }

        protected override SalesOrder BuildAggregate()
        {
            _order.AcceptUncommittedEvents();
            return _order;
        }

        public static SalesOrderTestDataBuilder NewOrder()
        {
            var salesOrder = new SalesOrder();
            salesOrder.Create(Guid.NewGuid());
            var builder = new SalesOrderTestDataBuilder(salesOrder);
            return builder;
        }

        public SalesOrderTestDataBuilder WithSingleLineItem()
        {
            _order.AddLine("SKU", 10);
            CurrentDescription = "Order with single line item";
            return this;
        }
    }
}