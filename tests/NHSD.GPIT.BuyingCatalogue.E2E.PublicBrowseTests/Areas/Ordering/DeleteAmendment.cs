﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class DeleteAmendment : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90030, 2);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public DeleteAmendment(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeleteOrderController), nameof(DeleteOrderController.DeleteOrder), Parameters)
        {
        }

        [Fact]
        public void DeleteAmendment_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.WarningCallout);
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.SelectOptions);
        }

        [Fact]
        public void DeleteOriginalOrder_Redirects()
        {
            var callOffId = new CallOffId(90030, 1);

            NavigateToUrl(
                typeof(DeleteOrderController),
                nameof(DeleteOrderController.DeleteOrder),
                new Dictionary<string, string>
                {
                    { nameof(InternalOrgId), InternalOrgId },
                    { nameof(CallOffId), callOffId.ToString() },
                });

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();
        }

        [Fact]
        public void DeleteAmendment_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void DeleteAmendment_SelectNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(DeleteOrderModel.AmendmentNoOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAmendment_SelectYes_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var order = await context.Orders
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

            order.Id = 0;
            order.Revision++;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            Dictionary<string, string> parameters = new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(DeleteOrderController),
                nameof(DeleteOrderController.DeleteOrder),
                parameters);

            CommonActions.ClickRadioButtonWithText(DeleteOrderModel.AmendmentYesOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DashboardController),
                    nameof(DashboardController.Organisation))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteAmendment_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteOrderController),
                nameof(DeleteOrderController.DeleteOrder)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DeleteOrderObjects.SelectOptionError).Should().BeTrue();
        }

        public override async Task DisposeAsync()
        {
            await using var context = GetEndToEndDbContext();

            var order = await context.Order(CallOffId);
            order.IsDeleted = false;

            await context.SaveChangesAsync();
            await base.DisposeAsync();
        }
    }
}
