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
    public class EditAssociatedServicesOnly : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90014;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingServiceId = new(99998, "S-997");
        private static readonly CatalogueItemId NewServiceId = new(99998, "S-999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public EditAssociatedServicesOnly(LocalWebApplicationFactory factory)
            : base(factory, typeof(AssociatedServicesController), nameof(AssociatedServicesController.EditAssociatedServices), Parameters)
        {
        }

        private OrderItem ExistingService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingServiceId);

        private OrderItem NewService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewServiceId);

        [Fact]
        public void EditAssociatedServices_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Associated Services - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.ServicesToSelect).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.NothingToSelect).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(3);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(1);
        }

        [Fact]
        public void EditAssociatedServices_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void EditAssociatedServices_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void EditAssociatedServices_AddAssociatedService_ExpectedResult()
        {
            CommonActions.ClickCheckboxByLabel("E2E Single Price Added Associated Service");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().NotBeNull();
        }

        [Fact]
        public void EditAssociatedServices_RemoveAssociatedService_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedServices)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AssociatedServicesObjects.SelectedServicesErrorMessage,
                $"Error:{SelectServicesModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void EditAssociatedServices_AddAndRemoveAssociatedService_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickCheckboxByLabel("E2E Single Price Added Associated Service");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.ConfirmAssociatedServiceChanges)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var orderItems = context.OrderItems
                .Include(x => x.CatalogueItem)
                .Where(x => x.OrderId == OrderId
                    && x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                    && x.CatalogueItemId != ExistingServiceId);

            context.OrderItems.RemoveRange(orderItems);
            context.SaveChanges();
        }
    }
}
