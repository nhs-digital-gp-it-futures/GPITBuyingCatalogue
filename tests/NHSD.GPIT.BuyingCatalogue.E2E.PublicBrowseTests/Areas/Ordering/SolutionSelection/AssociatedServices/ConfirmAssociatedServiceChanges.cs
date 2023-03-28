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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AssociatedServices
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmAssociatedServiceChanges : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 90008;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingServiceId = new(99998, "S-999");
        private static readonly CatalogueItemId NewServiceId = new(99998, "S-997");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "serviceIds", $"{NewServiceId}" },
        };

        public ConfirmAssociatedServiceChanges(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.ConfirmAssociatedServiceChanges),
                Parameters,
                queryParameters: QueryParameters)
        {
        }

        private OrderItem ExistingService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingServiceId);

        private OrderItem NewService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewServiceId);

        [Fact]
        public void ConfirmAssociatedServiceChanges_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Are you sure you want to change your Associated Services? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.RemovedItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.AddedItems).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmAssociatedServiceChanges_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedServices)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void ConfirmAssociatedServiceChanges_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.ConfirmAssociatedServiceChanges)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ConfirmServiceChangesObjects.ConfirmChangesError,
                $"Error:{ConfirmServiceChangesModelValidator.ErrorMessage}").Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void ConfirmAssociatedServiceChanges_ClickYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes, I want to confirm changes to my Associated Services");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ServiceRecipientsController),
                    nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            ExistingService.Should().BeNull();
            NewService.Should().NotBeNull();
        }

        [Fact]
        public void ConfirmAssociatedServiceChanges_ClickNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("No, I want to keep my current Associated Services");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var orderItems = context.OrderItems
                .Include(x => x.CatalogueItem)
                .Where(x => x.OrderId == OrderId
                    && x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);

            context.OrderItems.RemoveRange(orderItems);
            context.OrderItems.Add(new OrderItem
            {
                OrderId = OrderId,
                Created = DateTime.Now,
                CatalogueItem = context.CatalogueItems.First(x => x.Id == ExistingServiceId),
            });

            context.SaveChanges();
        }
    }
}
