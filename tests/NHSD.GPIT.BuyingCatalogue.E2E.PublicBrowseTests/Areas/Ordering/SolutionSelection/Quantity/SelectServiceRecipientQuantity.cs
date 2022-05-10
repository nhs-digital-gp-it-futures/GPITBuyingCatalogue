using System;
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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity
{
    public class SelectServiceRecipientQuantity : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90006;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectServiceRecipientQuantity(LocalWebApplicationFactory factory)
            : base(factory, typeof(QuantityController), nameof(QuantityController.SelectServiceRecipientQuantity), Parameters)
        {
        }

        [Fact]
        public void SelectQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Quantity of Catalogue Solution - E2E With Contact With Single Price".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            for (var i = 0; i < 3; i++)
            {
                CommonActions.ElementIsDisplayed(QuantityObjects.InputQuantityInput(i)).Should().BeTrue();
            }

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityIsBlank_Error()
        {
            CommonActions.ClearInputElement(QuantityObjects.InputQuantityInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.InputQuantityInputError(0),
                ServiceRecipientQuantityModelValidator.ValueNotEnteredErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityNotANumber_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(0), "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.InputQuantityInputError(0),
                ServiceRecipientQuantityModelValidator.ValueNotNumericErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityNegative_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(0), "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.InputQuantityInputError(0),
                ServiceRecipientQuantityModelValidator.ValueNegativeErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityHasDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(0), "1.1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                QuantityObjects.InputQuantityInputError(0),
                ServiceRecipientQuantityModelValidator.ValueNotAnIntegerErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SelectQuantity_QuantityIsValid_ExpectedResult()
        {
            for (var i = 0; i < 3; i++)
            {
                CommonActions.ElementAddValue(QuantityObjects.InputQuantityInput(i), "1234");
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedServices)).Should().BeTrue();

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
                .Where(x => x.OrderId == OrderId)
                .ToList();
        }
    }
}
