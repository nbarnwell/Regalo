using System;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class PlaceSalesOrder : Command
    {
        public Guid Id { get; private set; }

        public PlaceSalesOrder(Guid id)
        {
            Id = id;
        }
    }
}