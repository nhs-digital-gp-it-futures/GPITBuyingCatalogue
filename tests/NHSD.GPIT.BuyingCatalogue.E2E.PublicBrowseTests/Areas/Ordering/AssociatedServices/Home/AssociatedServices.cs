using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AssociatedServices
{
    public sealed class AssociatedServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90008, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public AssociatedServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AssociatedServicesController_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddItemButton("Associated Service")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AssociatedServicesController_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void AssociatedServicesController_ClickAddAssociatedService()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddItemButton("Associated Service"));

            CommonActions
                .PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServicesController_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AssociatedServicesController_ClickAssociatedService_GoToEditPage()
        {
            using var context = GetEndToEndDbContext();

            var catalogueItemName =
                 await context.Orders
                 .Include(o => o.OrderItems).ThenInclude(oi => oi.CatalogueItem)
                 .Where(o => o.Id == CallOffId.Id)
                 .Select(o => o.OrderItems[0].CatalogueItem.Name)
                 .SingleAsync();

            CommonActions.ClickLinkElement(By.LinkText(catalogueItemName));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();
        }
    }
}
