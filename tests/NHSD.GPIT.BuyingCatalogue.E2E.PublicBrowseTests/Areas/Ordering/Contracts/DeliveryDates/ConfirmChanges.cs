using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmChanges : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90009;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "deliveryDate", DateTime.Today.AddDays(2).ToString(DeliveryDatesController.DateFormat) },
        };

        public ConfirmChanges(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.ConfirmChanges), Parameters, queryParameters: QueryParameters)
        {
        }

        [Fact]
        public void ConfirmChanges_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Are you sure you want to change your planned delivery date? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.ConfirmChanges).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmChanges_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void ConfirmChanges_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.ConfirmChanges)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.ConfirmChangesError,
                $"Error: {ConfirmChangesModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void ConfirmChanges_SelectYes_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            var context = GetEndToEndDbContext();
            var expected = DateTime.Today.AddDays(2);

            context.Orders.First(x => x.Id == OrderId).DeliveryDate.Should().Be(expected);
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate.Should().Be(expected));
        }

        [Fact]
        public void ConfirmChanges_SelectNo_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.NoOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var tomorrow = DateTime.Today.AddDays(1);

            context.Orders.First(x => x.Id == OrderId).DeliveryDate = tomorrow;
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate = tomorrow);

            context.SaveChanges();
        }

        private void VerifyNoChangesMade()
        {
            var context = GetEndToEndDbContext();
            var tomorrow = DateTime.Today.AddDays(1);

            context.Orders.First(x => x.Id == OrderId).DeliveryDate.Should().Be(tomorrow);
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate.Should().Be(tomorrow));
        }
    }
}
