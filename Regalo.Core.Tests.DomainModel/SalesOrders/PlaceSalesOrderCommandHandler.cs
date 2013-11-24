namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class PlaceSalesOrderCommandHandler : ICommandHandler<PlaceSalesOrder>
    {
        public PlaceSalesOrderCommandHandler(IMessageHandlerContext<SalesOrder> context)
        {
            
        }

        public void Handle(PlaceSalesOrder command)
        {
            throw new System.NotImplementedException();
        }
    }
}