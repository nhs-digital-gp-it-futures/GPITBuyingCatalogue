using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AdditionalServices
{
    public class SelectAdditionalServiceRecipients : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90007;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002A999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public SelectAdditionalServiceRecipients(LocalWebApplicationFactory factory)
            : base(factory, typeof(ServiceRecipientsController), nameof(ServiceRecipientsController.AdditionalServiceRecipients), Parameters)
        {
        }

        [Fact]
        public void SelectAdditionalServiceRecipients_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Service Recipients - E2E No Contact Single Price Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.SelectAllLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.SelectNoneLink).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectAdditionalServiceRecipients_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SelectAdditionalServiceRecipients_ClickSelectAllLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ServiceRecipientObjects.SelectAllLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeFalse();
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(CommonActions.GetNumberOfCheckBoxesDisplayed());

            CommonActions.ClickLinkElement(ServiceRecipientObjects.SelectNoneLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeFalse();
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
        }

        [Fact]
        public void SelectAdditionalServiceRecipients_NoSelectionMade_DisplaysError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ServiceRecipientObjects.SelectedRecipientErrorMessage,
                $"Error:{SelectRecipientsModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void SelectAdditionalServiceRecipients_SelectionMade_ExpectedResult()
        {
            GetOrderItem().OrderItemRecipients.Count.Should().Be(0);

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.AdditionalServiceConfirmPrice)).Should().BeTrue();

            GetOrderItem().OrderItemRecipients.Count.Should().Be(1);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            foreach (var recipient in GetOrderItem().OrderItemRecipients)
            {
                context.OrderItemRecipients.Remove(recipient);
            }

            context.SaveChanges();
        }

        private OrderItem GetOrderItem()
        {
            var context = GetEndToEndDbContext();

            return context.OrderItems
                .Include(x => x.OrderItemRecipients)
                .Single(x => x.OrderId == OrderId
                    && x.CatalogueItemId == CatalogueItemId);
        }
    }
}
