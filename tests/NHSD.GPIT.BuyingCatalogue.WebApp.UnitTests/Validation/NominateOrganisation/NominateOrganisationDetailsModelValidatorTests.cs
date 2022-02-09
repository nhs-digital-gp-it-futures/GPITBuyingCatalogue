using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.NominateOrganisation
{
    public static class NominateOrganisationDetailsModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_OrganisationNameEmpty_ThrowsValidationError(
            string inputValue,
            NominateOrganisationDetailsModel model,
            NominateOrganisationDetailsModelValidator validator)
        {
            model.OrganisationName = inputValue;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrganisationName)
                .WithErrorMessage(NominateOrganisationDetailsModelValidator.OrganisationNameErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PrivacyPolicyNotChecked_ThrowsValidationError(
            NominateOrganisationDetailsModel model,
            NominateOrganisationDetailsModelValidator validator)
        {
            model.HasReadPrivacyPolicy = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HasReadPrivacyPolicy)
                .WithErrorMessage(NominateOrganisationDetailsModelValidator.HasReadPrivacyPolicyErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EverythingOk_NoErrors(
            NominateOrganisationDetailsModel model,
            NominateOrganisationDetailsModelValidator validator)
        {
            model.HasReadPrivacyPolicy = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
