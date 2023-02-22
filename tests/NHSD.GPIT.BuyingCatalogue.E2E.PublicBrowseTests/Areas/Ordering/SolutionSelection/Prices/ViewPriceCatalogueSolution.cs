using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class ViewPriceCatalogueSolution : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90030, 2);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");
        private static readonly RoutingSource Source = RoutingSource.TaskList;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
            { nameof(Source), $"{Source}" },
        };

        public ViewPriceCatalogueSolution(LocalWebApplicationFactory factory)
            : base(factory, typeof(PricesController), nameof(PricesController.ViewPrice), Parameters)
        {
        }

        [Fact]
        public void ViewPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Price of Catalogue Solution - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(PriceObjects.ViewPriceTiers).Should().BeTrue();
            CommonActions.ElementIsDisplayed(PriceObjects.ViewPriceDetails).Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ViewPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void ViewPrice_Continue_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();
        }
    }
}
