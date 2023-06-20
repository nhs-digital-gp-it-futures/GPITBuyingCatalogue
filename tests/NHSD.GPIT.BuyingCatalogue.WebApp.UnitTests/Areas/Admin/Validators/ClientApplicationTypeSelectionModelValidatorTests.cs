using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class ClientApplicationTypeSelectionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionWhenTypesAvailable_HasError(
            ApplicationTypeSelectionModel model,
            ApplicationTypeSelectionModelValidator validator)
        {
            model.ApplicationTypesAvailableForSelection = true;
            model.SelectedApplicationType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedApplicationType)
                .WithErrorMessage(ApplicationTypeSelectionModelValidator.SelectionErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionWhenTypesAvailable_DoesNotHaveError(
            ApplicationTypeSelectionModel model,
            ApplicationTypeSelectionModelValidator validator)
        {
            model.ApplicationTypesAvailableForSelection = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedApplicationType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionWhenNoTypesAvailable_DoesNotHaveError(
            ApplicationTypeSelectionModel model,
            ApplicationTypeSelectionModelValidator validator)
        {
            model.ApplicationTypesAvailableForSelection = false;
            model.SelectedApplicationType = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedApplicationType);
        }
    }
}
