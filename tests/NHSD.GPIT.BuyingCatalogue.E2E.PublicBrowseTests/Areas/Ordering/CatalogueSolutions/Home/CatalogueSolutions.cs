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
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutions
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90006, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public CatalogueSolutions(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutions_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddCatalogueSolution).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickGoBackLink_ExpectedResult()
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
        public void CatalogueSolutions_ClickAddSolution()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddCatalogueSolution);

            CommonActions
                .PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task CatalogueSolutions_ClickSolution_GoToEditPage()
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
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();
        }
    }
}
