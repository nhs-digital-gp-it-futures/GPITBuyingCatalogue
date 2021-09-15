using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
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
                  nameof(ListPriceController.Index),
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
                .ElementIsDisplayed(CommonSelectors.ContinueLink)
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
    }
}
