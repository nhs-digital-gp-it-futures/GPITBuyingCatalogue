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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90007, 1);

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(OdsCode), OdsCode }, { "CallOffId", CallOffId.ToString() } };

        public AdditionalServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalSolutions_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddAdditionalService).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AdditionalServices_ClickGoBackLink_ExpectedResult()
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
        public void AdditionalSolutions_ClickAddSolution()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddAdditionalService);

            CommonActions
                .PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalSolutions_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AdditionalSolutions_ClickSolution_GoToEditPage()
        {
            using var context = GetEndToEndDbContext();

            var orderItems =
                 await context.Orders
                 .Include(o => o.OrderItems).ThenInclude(oi => oi.CatalogueItem)
                 .Where(o => o.Id == CallOffId.Id)
                 .Select(o => o.OrderItems)
                 .SingleAsync();

            var catalogueItemName =
                orderItems
                .Where(oi => oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AdditionalService)
                .Select(oi => oi.CatalogueItem.Name).Single();

            CommonActions.ClickLinkElement(By.LinkText(catalogueItemName));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }
    }
}
