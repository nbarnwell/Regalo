using System;
using NUnit.Framework;
using Regalo.Core;
using Regalo.Core.Tests.DomainModel.SalesOrders;
using Regalo.ObjectCompare;

namespace Regalo.Testing.Tests.Unit
{
    [TestFixture]
    public class ApplicationServiceTestingTests : ApplicationServiceTestBase<SalesOrder>
    {
        [SetUp]
        public void SetUp()
        {
            Resolver.SetResolvers(
                type =>
                {
                    if (type == typeof(IVersionHandler))
                    {
                        return new DefaultVersionHandler();
                    }

                    if (type == typeof(ILogger))
                    {
                        return new ConsoleLogger();
                    }

                    throw new InvalidOperationException(string.Format("No resolver registered for {0}", type));
                },
                type =>
                {
                    return null;
                });

            ObjectComparisonResult.ThrowOnFail = true;
        }

        [TearDown]
        public void TearDown()
        {
            ObjectComparisonResult.ThrowOnFail = false;

            Resolver.ClearResolvers();
        }

        [Test]
        public void GivenSalesOrderWithSingleOrderLine_WhenPlacingOrder_ThenShouldPlaceOrder()
        {
            Scenario.For<SalesOrder>(Context)
                    .HandledBy<PlaceSalesOrderCommandHandler>(CreateHandler())
                    .Given(SalesOrderTestDataBuilder.NewOrder().WithSingleLineItem())
                    .When(c => new PlaceSalesOrder(c.Id))
                    .Then((a, c) => new[] { new SalesOrderPlaced(a.Id) })
                    .Assert();
        }

        [Test]
        public void GivenSalesOrderWithNoLines_WhenPlacingOrder_ThenShouldThrow()
        {
            Scenario.For(Context)
                    .HandledBy(CreateHandler())
                    .Given(SalesOrderTestDataBuilder.NewOrder())
                    .When(order => new PlaceSalesOrder(order.Id))
                    .Throws<InvalidOperationException>()
                    .Assert();
        }

        private PlaceSalesOrderCommandHandler CreateHandler()
        {
            return new PlaceSalesOrderCommandHandler(Context);
        }
    }
}