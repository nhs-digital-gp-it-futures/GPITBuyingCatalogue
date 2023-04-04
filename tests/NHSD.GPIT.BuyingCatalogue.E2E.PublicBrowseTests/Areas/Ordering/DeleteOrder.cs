using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class DeleteOrder
        : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public DeleteOrder(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(DeleteOrderController),
              nameof(DeleteOrderController.DeleteOrder),
              Parameters)
        {
        }

        [Fact]
        public void DeleteOrder_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.WarningCallout);
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.SelectOptions);
        }

        [Fact]
        public void DeleteOrder_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void DeleteOrder_SelectNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithValue("False");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void DeleteOrder_SelectYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithValue("True");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            using var context = GetEndToEndDbContext();

            context.Orders.Count(o => o.Id == CallOffId.OrderNumber).Should().Be(0);
        }

        [Fact]
        public void DeleteOrder_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteOrderController),
                nameof(DeleteOrderController.DeleteOrder)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DeleteOrderObjects.SelectOptionError).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var order = context.Orders.IgnoreQueryFilters().First(o => o.Id == CallOffId.OrderNumber);
            order.IsDeleted = false;

            context.SaveChanges();
        }
    }
}
