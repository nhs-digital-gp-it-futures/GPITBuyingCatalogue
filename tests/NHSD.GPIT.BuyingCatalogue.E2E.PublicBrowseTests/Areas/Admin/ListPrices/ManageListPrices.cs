using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices
{
    public sealed class ManageListPrices : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ManageListPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ListPriceController),
                  nameof(ListPriceController.ManageListPrices),
                  Parameters)
        {
        }

        [Fact]
        public void Index_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.ActionLink)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.ContinueLink)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.ListPriceTable)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Index_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Index_ClickAddPrice_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonObjects.ActionLink);

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ListPriceController),
                    nameof(ListPriceController.AddListPrice))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Index_ClickContinue_ExpectedResult()
        {
            CommonActions
                .ClickLinkElement(ListPricesObjects.ContinueLink);

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
            .Should()
            .BeTrue();
        }
    }
}
