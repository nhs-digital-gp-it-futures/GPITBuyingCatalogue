using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation;
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
                CommonActions.ElementIsDisplayed(NominateOrganisationObjects.OrganisationNameInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(NominateOrganisationObjects.OdsCodeInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.CheckboxItem).Should().BeTrue();
                CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(1);
                CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
                CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickGoBackLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_NoInput_ThrowsError()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Index)).Should().BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    NominateOrganisationObjects.OrganisationNameError,
                    NominateOrganisationDetailsModelValidator.OrganisationNameErrorMessage).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ValidInput_ExpectedResult()
        {
            RunTest(() =>
            {
                TextGenerators.TextInputAddText(NominateOrganisationObjects.OrganisationNameInput, 10);
                CommonActions.ClickCheckboxByLabel("I have read and understood the privacy policy");

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Confirmation)).Should().BeTrue();
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
