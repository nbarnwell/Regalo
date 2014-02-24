using System;
using System.Collections.Generic;

namespace Regalo.Core.Tests.DomainModel.SalesOrders
{
    public class SalesOrder : AggregateRoot
    {
        private readonly IDictionary<string, uint> _products = new Dictionary<string, uint>();

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
            if (false == OrderHasProducts()) throw new InvalidOperationException("Can't place an order with no products.");

            Record(new SalesOrderPlaced(Id));
        }

        private void Apply(SalesOrderCreated evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(ItemsAddedToOrder evt)
        {
            AddProduct(evt.Sku, evt.Quantity);
        }

        private bool OrderHasProducts()
        {
            return _products.Count > 0;
        }

        private bool OrderIncludesProduct(string sku)
        {
            return _products.ContainsKey(sku);
        }

        private void AddProduct(string sku, uint quantity)
        {
            if (false == OrderIncludesProduct(sku))
            {
                _products.Add(sku, quantity);
            }
            else
            {
                _products[sku] += quantity;
            }
        }
    }
}