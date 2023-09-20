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
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AdditionalServices
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmAdditionalServiceChanges : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90007;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingServiceId = new(99998, "001A99");
        private static readonly CatalogueItemId NewServiceId = new(99998, "001A98");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "serviceIds", $"{NewServiceId}" },
        };

        public ConfirmAdditionalServiceChanges(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.ConfirmAdditionalServiceChanges),
                Parameters,
                queryParameters: QueryParameters)
        {
        }

        private OrderItem ExistingService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingServiceId);

        private OrderItem NewService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewServiceId);

        [Fact]
        public void ConfirmAdditionalServiceChanges_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Are you sure you want to change your Additional Services? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.RemovedItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.AddedItems).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmAdditionalServiceChanges_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalServices)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void ConfirmAdditionalServiceChanges_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.ConfirmAdditionalServiceChanges)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ConfirmServiceChangesObjects.ConfirmChangesError,
                $"Error:{ConfirmServiceChangesModelValidator.ErrorMessage}").Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void ConfirmAdditionalServiceChanges_ClickYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes, I want to confirm changes to my Additional Services");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            ExistingService.Should().BeNull();
            NewService.Should().NotBeNull();
        }

        [Fact]
        public void ConfirmAdditionalServiceChanges_ClickNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("No, I want to keep my current Additional Services");
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
                    && x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);

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
