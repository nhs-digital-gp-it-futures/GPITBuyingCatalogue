using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity
{
    public class SelectQuantity : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90005;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectQuantity(LocalWebApplicationFactory factory)
            : base(factory, typeof(QuantityController), nameof(QuantityController.SelectQuantity), Parameters)
        {
        }

        [Fact]
        public void SelectQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Quantity of Catalogue Solution - DFOCVC Solution Full".FormatForComparison());
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

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedServices)).Should().BeTrue();

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
                .Where(x => x.OrderId == OrderId)
                .ToList();
        }
    }
}
