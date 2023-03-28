using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using CommonSelectors = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.CommonSelectors;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.HostingTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditOnPremiseHostingType : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public EditOnPremiseHostingType(LocalWebApplicationFactory factory)
           : base(
                 factory,
                 typeof(HostingTypesController),
                 nameof(HostingTypesController.Hybrid),
                 Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.HostingType_Summary)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.HostingType_Link)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.HostingType_HostingModel)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.CheckboxItem)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.DeleteHostingTypeButton)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HostingTypesController),
                    nameof(HostingTypesController.HostingType))
                .Should()
                .BeTrue();
        }
    }
}
