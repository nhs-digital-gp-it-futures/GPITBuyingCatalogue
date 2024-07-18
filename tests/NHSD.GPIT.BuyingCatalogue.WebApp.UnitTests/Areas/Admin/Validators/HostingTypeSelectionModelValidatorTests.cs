using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class HostingTypeSelectionModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionWhenTypesAvailable_HasError(
            HostingTypeSelectionModel model,
            HostingTypeSelectionModelValidator validator)
        {
            model.HostingTypesAvailableForSelection = true;
            model.SelectedHostingType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedHostingType)
                .WithErrorMessage(HostingTypeSelectionModelValidator.SelectionErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectionWhenTypesAvailable_DoesNotHaveError(
            HostingTypeSelectionModel model,
            HostingTypeSelectionModelValidator validator)
        {
            model.HostingTypesAvailableForSelection = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedHostingType);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionWhenNoTypesAvailable_DoesNotHaveError(
            HostingTypeSelectionModel model,
            HostingTypeSelectionModelValidator validator)
        {
            model.HostingTypesAvailableForSelection = false;
            model.SelectedHostingType = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedHostingType);
        }
    }
}
