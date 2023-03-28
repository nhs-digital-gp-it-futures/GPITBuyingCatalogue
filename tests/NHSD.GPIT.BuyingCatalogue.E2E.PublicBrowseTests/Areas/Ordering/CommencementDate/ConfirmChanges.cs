using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CommencementDate
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmChanges : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "IB-QWO";

        private const int OrderId = 90022;
        private const int OriginalInitialPeriod = 3;
        private const int OriginalMaximumTerm = 12;
        private const int NewInitialPeriod = 2;
        private const int NewMaximumTerm = 11;

        private static readonly DateTime OriginalCommencementDate = DateTime.Today.AddDays(2);
        private static readonly DateTime OriginalDeliveryDate = DateTime.Today.AddDays(3);
        private static readonly DateTime NewCommencementDate = DateTime.Today.AddDays(7);

        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly string Details = string.Join(
            CommencementDateController.Delimiter,
            NewCommencementDate.ToString(CommencementDateController.DateFormat),
            $"{NewInitialPeriod}",
            $"{NewMaximumTerm}");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
            { nameof(Details), Details },
        };

        public ConfirmChanges(LocalWebApplicationFactory factory)
            : base(factory, typeof(CommencementDateController), nameof(CommencementDateController.ConfirmChanges), Parameters)
        {
        }

        [Fact]
        public void ConfirmChanges_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Are you sure you want to change your commencement date? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.ConfirmChanges).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public void ConfirmChanges_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void ConfirmChanges_ClickCancelLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommencementDateObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void ConfirmChanges_NoSelectionMade_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.ConfirmChanges)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.ConfirmChangesError,
                $"Error:{ConfirmChangesModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void ConfirmChanges_YesSelected_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var context = GetEndToEndDbContext();

            var order = context.Orders.First(x => x.Id == OrderId);

            order.CommencementDate.Should().Be(NewCommencementDate);
            order.DeliveryDate.Should().BeNull();
            order.InitialPeriod.Should().Be(NewInitialPeriod);
            order.MaximumTerm.Should().Be(NewMaximumTerm);

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId)
                .ForEach(x => x.DeliveryDate.Should().BeNull());
        }

        [Fact]
        public void ConfirmChanges_NoSelected_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.NoOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var order = context.Orders.First(x => x.Id == OrderId);

            order.CommencementDate = OriginalCommencementDate;
            order.DeliveryDate = OriginalDeliveryDate;
            order.InitialPeriod = OriginalInitialPeriod;
            order.MaximumTerm = OriginalMaximumTerm;

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId)
                .ForEach(x => x.DeliveryDate = OriginalDeliveryDate);

            context.SaveChanges();
        }

        private void VerifyNoChangesMade()
        {
            var context = GetEndToEndDbContext();

            var order = context.Orders.First(x => x.Id == OrderId);

            order.CommencementDate.Should().Be(OriginalCommencementDate);
            order.DeliveryDate.Should().Be(OriginalDeliveryDate);
            order.InitialPeriod.Should().Be(OriginalInitialPeriod);
            order.MaximumTerm.Should().Be(OriginalMaximumTerm);

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId)
                .ForEach(x => x.DeliveryDate.Should().Be(OriginalDeliveryDate));
        }
    }
}
