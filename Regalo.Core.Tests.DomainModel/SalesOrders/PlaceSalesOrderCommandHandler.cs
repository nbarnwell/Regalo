using System;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class PlaceSalesOrderCommandHandler : ICommandHandler<PlaceSalesOrder>
    {
        private readonly IMessageHandlerContext<SalesOrder> _context;

        public PlaceSalesOrderCommandHandler(IMessageHandlerContext<SalesOrder> context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        public void Handle(PlaceSalesOrder command)
        {
            var order = _context.Get(command.Id);
            order.PlaceOrder();
            _context.SaveAndPublishEvents(order);
        }
    }
}