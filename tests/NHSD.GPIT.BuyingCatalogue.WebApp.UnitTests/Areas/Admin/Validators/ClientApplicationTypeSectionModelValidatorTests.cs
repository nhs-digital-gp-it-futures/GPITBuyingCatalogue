using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class ClientApplicationTypeSectionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoExistingHostingTypes_HasError(
            ClientApplicationTypeSectionModel model,
            ClientApplicationTypeSectionModelValidator validator)
        {
            model.ExistingApplicationTypesCount = 0;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ExistingApplicationTypesCount)
                .WithErrorMessage(ClientApplicationTypeSectionModelValidator.OneApplicationTypeRequiredMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ExistingHostingTypes_DoesNotHaveError(
            ClientApplicationTypeSectionModel model,
            ClientApplicationTypeSectionModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.ExistingApplicationTypesCount);
        }
    }
}
