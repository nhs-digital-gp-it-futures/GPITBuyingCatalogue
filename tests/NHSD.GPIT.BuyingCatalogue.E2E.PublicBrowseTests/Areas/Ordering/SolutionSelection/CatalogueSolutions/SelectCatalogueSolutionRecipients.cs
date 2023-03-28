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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    [Collection(nameof(OrderingCollection))]
    public class SelectCatalogueSolutionRecipients : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90005;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public SelectCatalogueSolutionRecipients(LocalWebApplicationFactory factory)
            : base(factory, typeof(ServiceRecipientsController), nameof(ServiceRecipientsController.AddServiceRecipients), Parameters)
        {
        }

        [Fact]
        public void SelectCatalogueSolutionRecipients_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Service Recipients for Catalogue Solution - DFOCVC Solution Full".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.SelectAllLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.SelectNoneLink).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolutionRecipients_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolutionRecipients_ClickSelectAllLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ServiceRecipientObjects.SelectAllLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeFalse();
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(CommonActions.GetNumberOfCheckBoxesDisplayed());

            CommonActions.ClickLinkElement(ServiceRecipientObjects.SelectNoneLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ServiceRecipientObjects.PreSelectedInset).Should().BeFalse();
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
        }

        [Fact]
        public void SelectCatalogueSolutionRecipients_NoSelectionMade_DisplaysError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ServiceRecipientObjects.SelectedRecipientErrorMessage,
                $"Error:{SelectRecipientsModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolutionRecipients_SelectionMade_ExpectedResult()
        {
            var context = GetEndToEndDbContext();
            var solution = GetSolution();

            var recipients = context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId
                    && x.CatalogueItemId == solution.CatalogueItemId)
                .ToList();

            context.OrderItemRecipients.Remove(recipients.First());

            context.SaveChanges();

            GetSolution().OrderItemRecipients.Count.Should().Be(2);

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.ConfirmChanges)).Should().BeTrue();

            GetSolution().OrderItemRecipients.Count.Should().Be(2);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var solution = GetSolution();

            var recipients = context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId
                    && x.CatalogueItemId == solution.CatalogueItemId)
                .ToList();

            foreach (var recipient in recipients)
            {
                context.OrderItemRecipients.Remove(recipient);
            }

            context.SaveChanges();
        }

        private OrderItem GetSolution()
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
                .First(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }
    }
}
