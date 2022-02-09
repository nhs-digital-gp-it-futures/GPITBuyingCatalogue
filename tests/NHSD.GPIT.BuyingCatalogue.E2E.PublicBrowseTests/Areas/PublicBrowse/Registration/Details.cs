using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Registration
{
    public sealed class Details : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string PrivacyPolicyLabelText = "I have read and understood the privacy policy";

        public Details(LocalWebApplicationFactory factory)
            : base(factory, typeof(RegistrationController), nameof(RegistrationController.Details), null)
        {
        }

        [Fact]
        public void Details_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.FullNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.TelephoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.EmailAddressInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.OrganisationNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.OdsCodeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.CheckboxItem).Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(2);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Details)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.FullNameError,
                RegistrationDetailsModelValidator.FullNameErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.TelephoneNumberError,
                RegistrationDetailsModelValidator.TelephoneNumberErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.EmailAddressError,
                RegistrationDetailsModelValidator.EmailAddressMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.OrganisationNameError,
                RegistrationDetailsModelValidator.OrganisationNameErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.HasReadPrivacyPolicyError,
                $"Error:{RegistrationDetailsModelValidator.PrivacyPolicyErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void Details_InvalidEmailAddress_ThrowsError()
        {
            TextGenerators.TextInputAddText(RegistrationObjects.FullNameInput, 10);
            TextGenerators.TextInputAddText(RegistrationObjects.TelephoneNumberInput, 10);
            TextGenerators.TextInputAddText(RegistrationObjects.EmailAddressInput, 10);
            TextGenerators.TextInputAddText(RegistrationObjects.OrganisationNameInput, 10);
            CommonActions.ClickCheckboxByLabel(PrivacyPolicyLabelText);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Details)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                RegistrationObjects.EmailAddressError,
                RegistrationDetailsModelValidator.EmailAddressWrongFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void Details_ValidInput_ExpectedResult()
        {
            TextGenerators.TextInputAddText(RegistrationObjects.FullNameInput, 10);
            TextGenerators.TextInputAddText(RegistrationObjects.TelephoneNumberInput, 10);
            TextGenerators.EmailInputAddText(RegistrationObjects.EmailAddressInput, 50);
            TextGenerators.TextInputAddText(RegistrationObjects.OrganisationNameInput, 10);
            CommonActions.ClickCheckboxByLabel(PrivacyPolicyLabelText);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Confirmation)).Should().BeTrue();
        }
    }
}
