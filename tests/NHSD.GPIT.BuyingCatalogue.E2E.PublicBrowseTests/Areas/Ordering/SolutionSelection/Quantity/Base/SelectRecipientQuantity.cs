using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity.Base
{
    [Collection(nameof(OrderingCollection))]
    public abstract class SelectRecipientQuantity : BuyerTestBase, IDisposable
    {
        private const int ServiceRecipientIndex = 0;
        private readonly int orderId;
        private readonly CatalogueItemId catalogueItemId;

        protected SelectRecipientQuantity(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(
                factory,
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity),
                parameters)
        {
            orderId = int.Parse(parameters["OrderId"]);
            catalogueItemId = CatalogueItemId.ParseExact(parameters["CatalogueItemId"]);
        }

        private string ServiceRecipientName => GetServiceRecipientName();

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
            CommonActions.ClearInputElement(QuantityObjects.InputQuantityInput(ServiceRecipientIndex));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(QuantityController),
                    nameof(QuantityController.SelectServiceRecipientQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                    QuantityObjects.InputQuantityInputError(ServiceRecipientIndex),
                    string.Format(
                        ServiceRecipientQuantityModelValidator.ValueNotEnteredErrorMessage,
                        ServiceRecipientName))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void SelectServiceRecipientQuantity_QuantityNotANumber_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(ServiceRecipientIndex), "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(QuantityController),
                    nameof(QuantityController.SelectServiceRecipientQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                    QuantityObjects.InputQuantityInputError(ServiceRecipientIndex),
                    string.Format(
                        ServiceRecipientQuantityModelValidator.ValueNotNumericErrorMessage,
                        ServiceRecipientName))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void SelectServiceRecipientQuantity_QuantityNegative_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(ServiceRecipientIndex), "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(QuantityController),
                    nameof(QuantityController.SelectServiceRecipientQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                    QuantityObjects.InputQuantityInputError(ServiceRecipientIndex),
                    string.Format(
                        ServiceRecipientQuantityModelValidator.ValueNegativeErrorMessage,
                        ServiceRecipientName))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void SelectServiceRecipientQuantity_QuantityHasDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(ServiceRecipientIndex), "1.1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(QuantityController),
                    nameof(QuantityController.SelectServiceRecipientQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                    QuantityObjects.InputQuantityInputError(ServiceRecipientIndex),
                    string.Format(
                        ServiceRecipientQuantityModelValidator.ValueNotAnIntegerErrorMessage,
                        ServiceRecipientName))
                .Should()
                .BeTrue();
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

        private string GetServiceRecipientName() =>
            Driver.FindElement(QuantityObjects.InputHiddenServiceRecipientName(ServiceRecipientIndex))
                .GetAttribute("Value");

        private List<OrderItem> GetOrderItems()
        {
            return GetEndToEndDbContext()
                .OrderItems
                .Include(x => x.OrderItemRecipients)
                .Where(
                    x => x.OrderId == orderId
                        && x.CatalogueItemId == catalogueItemId)
                .ToList();
        }
    }
}
