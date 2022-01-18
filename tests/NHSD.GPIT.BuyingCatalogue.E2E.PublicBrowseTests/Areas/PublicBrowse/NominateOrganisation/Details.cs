using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.NominateOrganisation
{
    public sealed class Details : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Details(LocalWebApplicationFactory factory)
            : base(factory, typeof(NominateOrganisationController), nameof(NominateOrganisationController.Details), null)
        {
        }

        [Fact]
        public void Details_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominateOrganisationObjects.OrganisationNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NominateOrganisationObjects.OdsCodeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.CheckboxItem).Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(1);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(NominateOrganisationController),
                nameof(NominateOrganisationController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(NominateOrganisationController),
                nameof(NominateOrganisationController.Details)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NominateOrganisationObjects.OrganisationNameError,
                NominateOrganisationDetailsModelValidator.OrganisationNameErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NominateOrganisationObjects.HasReadPrivacyPolicyError,
                $"Error:{NominateOrganisationDetailsModelValidator.HasReadPrivacyPolicyErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void Details_ValidInput_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NominateOrganisationObjects.OrganisationNameInput, 10);
            CommonActions.ClickCheckboxByLabel("I have read and understood the privacy policy");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(NominateOrganisationController),
                nameof(NominateOrganisationController.Confirmation)).Should().BeTrue();
        }

        public void Dispose()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));
        }
    }
}
