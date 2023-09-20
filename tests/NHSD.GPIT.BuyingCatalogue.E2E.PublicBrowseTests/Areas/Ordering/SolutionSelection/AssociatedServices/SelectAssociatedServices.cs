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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AssociatedServices
{
    [Collection(nameof(OrderingCollection))]
    public class SelectAssociatedServices : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectAssociatedServices(LocalWebApplicationFactory factory)
            : base(factory, typeof(AssociatedServicesController), nameof(AssociatedServicesController.SelectAssociatedServices), Parameters)
        {
        }

        [Fact]
        public void SelectAssociatedServices_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Associated Services - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.ServicesToSelect).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.NothingToSelect).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectAssociatedServices_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SelectAssociatedServices_WithSomeExistingServices_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            NavigateToUrl(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices),
                Parameters);

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(1);
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.ServicesToSelect).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.NothingToSelect).Should().BeFalse();
        }

        [Fact]
        public void SelectAssociatedServices_WithAllExistingServices_ExpectedResult()
        {
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();

            NavigateToUrl(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices),
                Parameters);

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(3);
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.ServicesToSelect).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.NothingToSelect).Should().BeFalse();
        }

        [Fact]
        public void SelectAssociatedServices_NoSelectionMade_AssociatedServicesOnly_ExpectedResult()
        {
            var context = GetEndToEndDbContext();

            var order = context.Orders.First(x => x.Id == OrderId);

            order.AssociatedServicesOnly = true;
            order.SolutionId = new CatalogueItemId(99998, "001");

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AssociatedServicesObjects.SelectedServicesErrorMessage,
                $"Error:{SelectServicesModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void SelectAssociatedServices_NoSelectionMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ReviewSolutionsController),
                nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();
        }

        [Fact]
        public void SelectAssociatedServices_SelectionMade_ExpectedResult()
        {
            GetAssociatedServices().Should().BeEmpty();

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            GetAssociatedServices().Count.Should().Be(1);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            foreach (var service in GetAssociatedServices())
            {
                context.OrderItems.Remove(service);
            }

            context.Orders.First(x => x.Id == OrderId).AssociatedServicesOnly = false;
            context.Orders.First(x => x.Id == OrderId).SolutionId = null;

            context.SaveChanges();
        }

        private List<OrderItem> GetAssociatedServices()
        {
            var context = GetEndToEndDbContext();

            return context.OrderItems
                .Include(x => x.CatalogueItem)
                .Where(x => x.OrderId == OrderId
                    && x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                .ToList();
        }
    }
}
