using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public class AmendOrder : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90010, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public AmendOrder(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrderController), nameof(OrderController.AmendOrder), Parameters)
        {
        }

        [Fact]
        public void AmendOrder_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton);
            CommonActions.ElementIsDisplayed(AmendOrderObjects.CancelLink);
            CommonActions.ElementIsDisplayed(AmendOrderObjects.ProcurementSupportLink);
        }

        [Fact]
        public void AmendOrder_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            GetNextRevision().Should().BeNull();
        }

        [Fact]
        public void AmendOrder_ClickCancel_ExpectedResult()
        {
            CommonActions.ClickLinkElement(AmendOrderObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            GetNextRevision().Should().BeNull();
        }

        [Fact]
        public void AmendOrder_ClickSubmit_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            GetNextRevision().Should().NotBeNull();
        }

        [Fact]
        public void AmendOrder_ClickProcurementSupportLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(AmendOrderObjects.ProcurementSupportLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            GetNextRevision().Should().BeNull();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var orders = context.Orders.IgnoreQueryFilters()
                .Where(x => x.OrderNumber == CallOffId.OrderNumber
                    && x.Revision > CallOffId.Revision)
                .ToList();

            if (!orders.Any())
            {
                return;
            }

            context.Orders.RemoveRange(orders);
            context.SaveChanges();
        }

        private Order GetNextRevision() => GetEndToEndDbContext().Orders
            .FirstOrDefault(x => x.OrderNumber == CallOffId.OrderNumber
                && x.Revision == CallOffId.Revision + 1);
    }
}
