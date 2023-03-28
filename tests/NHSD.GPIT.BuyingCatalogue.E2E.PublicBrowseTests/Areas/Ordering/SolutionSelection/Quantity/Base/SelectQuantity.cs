using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity.Base
{
    [Collection(nameof(OrderingCollection))]
    public abstract class SelectQuantity : BuyerTestBase, IDisposable
    {
        private readonly int orderId;
        private readonly CatalogueItemId catalogueItemId;

        protected SelectQuantity(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(QuantityController), nameof(QuantityController.SelectQuantity), parameters)
        {
            orderId = int.Parse(parameters["OrderId"]);
            catalogueItemId = CatalogueItemId.ParseExact(parameters["CatalogueItemId"]);
        }

        protected abstract string PageTitle { get; }

        protected abstract Type OnwardController { get; }

        protected abstract string OnwardActionName { get; }

        [Fact]
        public void SelectQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(PageTitle.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(QuantityObjects.QuantityInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityIsBlank_Error()
        {
            CommonActions.ClearInputElement(QuantityObjects.QuantityInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.QuantityInputError,
                SelectOrderItemQuantityModelValidator.QuantityNotEnteredErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityNotANumber_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.QuantityInput, "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.QuantityInputError,
                SelectOrderItemQuantityModelValidator.QuantityNotANumberErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityNegative_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.QuantityInput, "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.QuantityInputError,
                SelectOrderItemQuantityModelValidator.QuantityNegativeErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityHasDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.QuantityInput, "1.1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.QuantityInputError,
                SelectOrderItemQuantityModelValidator.QuantityNotAWholeNumberErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityIsValid_ExpectedResult()
        {
            CommonActions.ElementAddValue(QuantityObjects.QuantityInput, "1234");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(OnwardController, OnwardActionName).Should().BeTrue();

            var orderItems = GetOrderItems();

            orderItems.Count.Should().Be(1);
            orderItems.First().Quantity.Should().Be(1234);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var orderItems = GetOrderItems();

            orderItems.ForEach(x => x.Quantity = null);

            context.UpdateRange(orderItems);
            context.SaveChanges();
        }

        private List<OrderItem> GetOrderItems()
        {
            return GetEndToEndDbContext().OrderItems
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .ToList();
        }
    }
}
