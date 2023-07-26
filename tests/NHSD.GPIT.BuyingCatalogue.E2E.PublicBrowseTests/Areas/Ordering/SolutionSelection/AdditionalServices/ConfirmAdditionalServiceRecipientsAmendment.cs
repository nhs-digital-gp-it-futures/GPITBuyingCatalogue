using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AdditionalServices
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmAdditionalServiceRecipientsAmendment : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90031;
        private const string RecipientIds = "Y03508,Y07021";
        private const JourneyType Journey = JourneyType.Edit;
        private const RoutingSource Source = RoutingSource.TaskList;
        private static readonly CallOffId CallOffId = new(OrderId, 2);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { nameof(RecipientIds), $"{RecipientIds}" },
            { nameof(Journey), $"{Journey}" },
            { nameof(Source), $"{Source}" },
        };

        public ConfirmAdditionalServiceRecipientsAmendment(LocalWebApplicationFactory factory)
            : base(factory, typeof(ServiceRecipientsController), nameof(ServiceRecipientsController.ConfirmChanges), Parameters, queryParameters: QueryParameters)
        {
        }

        [Fact]
        public void ConfirmAdditionalServiceRecipientsAmendment_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(x => x.InternalIdentifier == InternalOrgId);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Confirm Service Recipients for {organisation.Name} ({organisation.ExternalIdentifier}) - E2E Multiple Prices Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.AddOrRemoveLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreviouslySelectedServiceRecipients).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.SelectedServiceRecipients).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmAdditionalServiceRecipientsAmendment_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(2);
        }

        [Fact]
        public void ConfirmAdditionalServiceRecipientsAmendment_ClickAddOrRemoveLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ServiceRecipientObjects.AddOrRemoveLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.EditServiceRecipients)).Should().BeTrue();

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(2);
        }

        [Fact]
        public void ConfirmAdditionalServiceRecipientsAmendment_ClickContinue_ExpectedResult()
        {
            GetService().OrderItemRecipients.Should().BeEmpty();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ViewPrice)).Should().BeTrue();

            GetService().OrderItemRecipients.Count.Should().Be(2);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var orderId = context.OrderId(InternalOrgId, CallOffId).Result;

            var recipients = context.OrderItemRecipients
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == CatalogueItemId)
                .ToList();

            foreach (var recipient in recipients)
            {
                context.OrderItemRecipients.Remove(recipient);
            }

            context.SaveChanges();
        }

        private OrderItem GetService()
        {
            var context = GetEndToEndDbContext();

            var order = context.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.OrderItems).ThenInclude(x => x.CatalogueItem)
                .Include(x => x.OrderItems).ThenInclude(x => x.OrderItemRecipients)
                .First(x => x.OrderNumber == CallOffId.OrderNumber
                    && x.Revision == CallOffId.Revision
                    && x.OrderingParty.InternalIdentifier == InternalOrgId);

            return order.OrderItems
                .First(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);
        }
    }
}
