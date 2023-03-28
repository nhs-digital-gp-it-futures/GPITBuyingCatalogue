using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.ProcurementHub;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.ProcurementHub
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class Index : AnonymousTestBase
    {
        private const string PrivacyPolicyLabelText = "I have read and understood the privacy policy";

        public Index(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                factory,
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index),
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
                CommonActions.ElementIsDisplayed(ProcurementHubObjects.FullNameInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ProcurementHubObjects.EmailAddressInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ProcurementHubObjects.OrganisationNameInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ProcurementHubObjects.OdsCodeInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ProcurementHubObjects.QueryInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.CheckboxItem).Should().BeTrue();
                CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(1);
                CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
                CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_WithLoggedInUser_UserFieldsPopulated()
        {
            RunTest(() =>
            {
                var context = GetEndToEndDbContext();

                var user = GetBuyer();
                var organisation = context.Organisations.First(x => x.Id == user.PrimaryOrganisationId);

                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                AuthorizationPages.LoginActions.Login(user.Email, DefaultPassword);

                NavigateToUrl(
                    typeof(ProcurementHubController),
                    nameof(ProcurementHubController.Index));

                CommonActions.ElementTextEqualTo(ProcurementHubObjects.FullNameInput, user.FullName);
                CommonActions.ElementTextEqualTo(ProcurementHubObjects.EmailAddressInput, user.Email);
                CommonActions.ElementTextEqualTo(ProcurementHubObjects.OrganisationNameInput, organisation.Name);
                CommonActions.ElementTextEqualTo(ProcurementHubObjects.FullNameInput, organisation.ExternalIdentifier);
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
        public void Index_NoInput_ThrowsError()
        {
            RunTest(() =>
            {
                CommonActions.ElementAddValue(ProcurementHubObjects.FullNameInput, string.Empty);
                CommonActions.ElementAddValue(ProcurementHubObjects.EmailAddressInput, string.Empty);
                CommonActions.ElementAddValue(ProcurementHubObjects.OrganisationNameInput, string.Empty);
                CommonActions.ElementAddValue(ProcurementHubObjects.OdsCodeInput, string.Empty);

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ProcurementHubController),
                    nameof(ProcurementHubController.Index)).Should().BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.FullNameError,
                    ProcurementHubDetailsModelValidator.FullNameMissingErrorMessage).Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.EmailAddressError,
                    ProcurementHubDetailsModelValidator.EmailAddressMissingErrorMessage).Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.OrganisationNameError,
                    ProcurementHubDetailsModelValidator.OrganisationNameMissingErrorMessage).Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.QueryError,
                    ProcurementHubDetailsModelValidator.QueryMissingErrorMessage).Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.HasReadPrivacyPolicyError,
                    $"Error:{ProcurementHubDetailsModelValidator.PrivacyPolicyErrorMessage}").Should().BeTrue();
            });
        }

        [Fact]
        public void Index_InvalidEmailAddress_ThrowsError()
        {
            RunTest(() =>
            {
                TextGenerators.TextInputAddText(ProcurementHubObjects.FullNameInput, 10);
                TextGenerators.TextInputAddText(ProcurementHubObjects.EmailAddressInput, 10);
                TextGenerators.TextInputAddText(ProcurementHubObjects.OrganisationNameInput, 10);
                CommonActions.ClickCheckboxByLabel(PrivacyPolicyLabelText);

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ProcurementHubController),
                    nameof(ProcurementHubController.Index)).Should().BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

                CommonActions.ElementShowingCorrectErrorMessage(
                    ProcurementHubObjects.EmailAddressError,
                    ProcurementHubDetailsModelValidator.EmailAddressWrongFormatErrorMessage).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ValidInput_ExpectedResult()
        {
            RunTest(() =>
            {
                TextGenerators.TextInputAddText(ProcurementHubObjects.FullNameInput, 10);
                TextGenerators.EmailInputAddText(ProcurementHubObjects.EmailAddressInput, 50);
                TextGenerators.TextInputAddText(ProcurementHubObjects.OrganisationNameInput, 10);
                TextGenerators.TextInputAddText(ProcurementHubObjects.QueryInput, 10);
                CommonActions.ClickCheckboxByLabel(PrivacyPolicyLabelText);

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ProcurementHubController),
                    nameof(ProcurementHubController.Confirmation)).Should().BeTrue();
            });
        }
    }
}
