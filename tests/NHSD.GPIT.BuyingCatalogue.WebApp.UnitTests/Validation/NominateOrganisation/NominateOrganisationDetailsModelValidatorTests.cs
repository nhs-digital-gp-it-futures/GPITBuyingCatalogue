using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.NominateOrganisation
{
    public static class NominateOrganisationDetailsModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockInlineAutoData]
        public static void Validate_EverythingOk_NoErrors(
            NominateOrganisationDetailsModel model,
            NominateOrganisationDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
