using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AdditionalServices
{
    public static class SelectAdditionalServiceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedAdditionalServiceNull_SetsModelError(
            SelectAdditionalServiceModel model,
            SelectAdditionalServiceModelValidator validator)
        {
            model.SelectedAdditionalServiceId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAdditionalServiceId)
                .WithErrorMessage("Select an Additional Service");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectAdditionalServiceModel model,
            SelectAdditionalServiceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
