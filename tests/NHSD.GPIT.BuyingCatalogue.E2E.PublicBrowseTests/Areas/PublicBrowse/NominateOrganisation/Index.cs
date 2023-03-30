using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.NominateOrganisation
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class Index : BuyerTestBase, IDisposable
    {
        public Index(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                factory,
                typeof(NominateOrganisationController),
                nameof(NominateOrganisationController.Index),
                null,
                testOutputHelper)
        {
        }

        [Fact]
        public void Index_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
                CommonActions.ElementIsDisplayed(NominateOrganisationObjects.ProcurementHubLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(NominateOrganisationObjects.NominateAnOrganisationLink).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickGoBackLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickProcurementHubLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(NominateOrganisationObjects.ProcurementHubLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ProcurementHubController),
                    nameof(ProcurementHubController.Index)).Should().BeTrue();

                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickNominateOrganisationLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(NominateOrganisationObjects.NominateAnOrganisationLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Details)).Should().BeTrue();
            });
        }

        public void Dispose()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));
        }
    }
}
