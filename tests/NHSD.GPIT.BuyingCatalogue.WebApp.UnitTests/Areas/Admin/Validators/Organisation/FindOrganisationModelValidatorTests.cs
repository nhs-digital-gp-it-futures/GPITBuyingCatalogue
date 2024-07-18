using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Organisation
{
    public static class FindOrganisationModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoOdsCode_SetsModelError(
            FindOrganisationModel model,
            FindOrganisationModelValidator validator)
        {
            model.OdsCode = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OdsCode)
                .WithErrorMessage("Enter ODS code");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_OdsCode_SetsModelError(
            string odsCode,
            FindOrganisationModel model,
            FindOrganisationModelValidator validator)
        {
            model.OdsCode = odsCode;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
