using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Validators.Filters
{
    public static class IncludeEpicsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            IncludeEpicsModel model,
            IncludeEpicsModelValidator validator)
        {
            model.IncludeEpics = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.IncludeEpics)
                .WithErrorMessage(IncludeEpicsModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_SelectionMade_NoErrors(
            bool value,
            IncludeEpicsModel model,
            IncludeEpicsModelValidator validator)
        {
            model.IncludeEpics = value;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
