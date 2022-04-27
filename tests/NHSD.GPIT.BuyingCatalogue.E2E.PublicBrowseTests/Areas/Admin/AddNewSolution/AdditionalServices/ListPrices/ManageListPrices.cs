using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices.ListPrices
{
    public sealed class ManageListPrices : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public ManageListPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.ManageListPrices),
                  Parameters)
        {
        }

        [Fact]
        public void ManageListPrices_AllSectionsDisplayed()
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
        public void ManageListPrices_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageListPrices_ClickAddPrice_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonObjects.ActionLink);

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.AddListPrice))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageListPrices_ClickContinue_ExpectedResult()
        {
            CommonActions
                .ClickLinkElement(ListPricesObjects.ContinueLink);

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
            .Should()
            .BeTrue();
        }
    }
}
