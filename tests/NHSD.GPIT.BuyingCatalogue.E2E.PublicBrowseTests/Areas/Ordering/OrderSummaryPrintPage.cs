using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderSummaryPrintPage
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public OrderSummaryPrintPage(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderSummaryController),
                  nameof(OrderSummaryController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void Index_CompletedOrder_PageTitleAsExpected()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Single(o => o.Id == CallOffId.Id);
            var originalOrderStatus = order.OrderStatus;
            order.Complete();

            context.Orders.Update(order);
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.PageTitle().Should().Be($"Call-off Order Form - Order completed for {CallOffId}".FormatForComparison());
            CommonActions.LedeText().Should().Be("This order has been completed and can no longer be changed.".FormatForComparison());

            order.OrderStatus = originalOrderStatus;
            context.SaveChanges();
        }
    }
}
