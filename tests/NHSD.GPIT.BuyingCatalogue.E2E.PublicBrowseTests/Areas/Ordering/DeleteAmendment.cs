﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class DeleteAmendment : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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
        public void DeleteAmendment_SelectYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(DeleteOrderModel.AmendmentYesOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            using var context = GetEndToEndDbContext();

            context.Orders.Count(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision).Should().Be(0);
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

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var orderId = context.OrderId(CallOffId).Result;

            context.Database.ExecuteSqlInterpolated($"UPDATE Orders SET IsDeleted = 0 WHERE Id = {orderId}");
        }
    }
}
