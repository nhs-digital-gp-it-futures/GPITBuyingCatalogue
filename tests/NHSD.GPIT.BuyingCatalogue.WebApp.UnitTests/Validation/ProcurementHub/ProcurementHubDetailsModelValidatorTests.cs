using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.ProcurementHub;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.ProcurementHub
{
    public static class ProcurementHubDetailsModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_AllPropertiesEmpty_ThrowsValidationError(
            string inputValue,
            ProcurementHubDetailsModel model,
            ProcurementHubDetailsModelValidator validator)
        {
            model.EmailAddress = inputValue;
            model.FullName = inputValue;
            model.OrganisationName = inputValue;
            model.Query = inputValue;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.EmailAddressMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.FullName)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.FullNameMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.OrganisationName)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.OrganisationNameMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.Query)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.QueryMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailAddressWrongFormat_ThrowsValidationError(
            ProcurementHubDetailsModel model,
            ProcurementHubDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.EmailAddressWrongFormatErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PrivacyPolicyNotChecked_ThrowsValidationError(
            ProcurementHubDetailsModel model,
            ProcurementHubDetailsModelValidator validator)
        {
            model.HasReadPrivacyPolicy = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HasReadPrivacyPolicy)
                .WithErrorMessage(ProcurementHubDetailsModelValidator.PrivacyPolicyErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EverythingOk_NoErrors(
            ProcurementHubDetailsModel model,
            ProcurementHubDetailsModelValidator validator)
        {
            model.EmailAddress = "a@b.com";
            model.HasReadPrivacyPolicy = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
