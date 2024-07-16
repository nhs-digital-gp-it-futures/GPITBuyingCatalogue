using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class HostingTypeSectionModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoExistingHostingTypes_HasError(
            HostingTypeSectionModel model,
            HostingTypeSectionModelValidator validator)
        {
            model.ExistingHostingTypesCount = 0;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ExistingHostingTypesCount)
                .WithErrorMessage(HostingTypeSectionModelValidator.OneHostingTypeRequiredMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ExistingHostingTypes_DoesNotHaveError(
            HostingTypeSectionModel model,
            HostingTypeSectionModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.ExistingHostingTypesCount);
        }
    }
}
