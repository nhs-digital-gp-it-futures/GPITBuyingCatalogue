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
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AdditionalServices
{
    [Collection(nameof(OrderingCollection))]
    public class EditAdditionalServices : BuyerTestBase, IDisposable
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

        public EditAdditionalServices(LocalWebApplicationFactory factory)
            : base(factory, typeof(AdditionalServicesController), nameof(AdditionalServicesController.SelectAdditionalServices), Parameters)
        {
        }

        private OrderItem ExistingService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingServiceId);

        private OrderItem NewService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewServiceId);

        [Fact]
        public void EditAdditionalServices_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Additional Services - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdditionalServicesObjects.ServicesToSelect).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdditionalServicesObjects.NothingToSelect).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(2);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(1);
        }

        [Fact]
        public void EditAdditionalServices_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void EditAdditionalServices_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().BeNull();
        }

        [Fact]
        public void EditAdditionalServices_AddAdditionalService_ExpectedResult()
        {
            CommonActions.ClickCheckboxByLabel("E2E Single Price Additional Service");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            ExistingService.Should().NotBeNull();
            NewService.Should().NotBeNull();
        }

        [Fact]
        public void EditAdditionalServices_RemoveAdditionalService_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();
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
                    && x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                    && x.CatalogueItemId != ExistingServiceId);

            context.OrderItems.RemoveRange(orderItems);
            context.SaveChanges();
        }
    }
}
