﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity.Base
{
    public abstract class SelectRecipientQuantity : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private readonly int orderId;
        private readonly CatalogueItemId catalogueItemId;

        protected SelectRecipientQuantity(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(QuantityController), nameof(QuantityController.SelectServiceRecipientQuantity), parameters)
        {
            orderId = int.Parse(parameters["OrderId"]);
            catalogueItemId = CatalogueItemId.ParseExact(parameters["CatalogueItemId"]);
        }

        protected abstract string PageTitle { get; }

        protected abstract Type OnwardController { get; }

        protected abstract string OnwardActionName { get; }

        [Fact]
        public void SelectServiceRecipientQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(PageTitle.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            for (var i = 0; i < 3; i++)
            {
                CommonActions.ElementIsDisplayed(QuantityObjects.InputQuantityInput(i)).Should().BeTrue();
            }

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectServiceRecipientQuantity_QuantityIsBlank_Error()
        {
            CommonActions.ClearInputElement(QuantityObjects.InputQuantityInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(QuantityObjects.InputQuantityInputError(0)).Should().BeTrue();
        }

        [Fact]
        public void SelectServiceRecipientQuantity_QuantityIsValid_ExpectedResult()
        {
            for (var i = 0; i < 3; i++)
            {
                CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(i), "1234");
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(OnwardController, OnwardActionName).Should().BeTrue();

            var orderItems = GetOrderItems();

            orderItems.Count.Should().Be(1);

            foreach (var recipient in orderItems[0].OrderItemRecipients)
            {
                recipient.Quantity.Should().Be(1234);
            }
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var orderItems = GetOrderItems();

            orderItems.ForEach(x => x.OrderItemRecipients.ToList().ForEach(r => r.Quantity = null));

            context.UpdateRange(orderItems);
            context.SaveChanges();
        }

        private List<OrderItem> GetOrderItems()
        {
            return GetEndToEndDbContext().OrderItems
                .Include(x => x.OrderItemRecipients)
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .ToList();
        }
    }
}
