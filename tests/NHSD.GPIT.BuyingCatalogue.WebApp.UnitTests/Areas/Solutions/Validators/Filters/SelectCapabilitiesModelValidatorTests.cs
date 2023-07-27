using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Validators.Filters
{
    public static class SelectCapabilitiesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectCapabilitiesModel model,
            SelectCapabilitiesModelValidator validator)
        {
            model.SelectedItems.ForEach(x => x.Selected = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(SelectCapabilitiesModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionMade_NoErrors(
            SelectCapabilitiesModel model,
            SelectCapabilitiesModelValidator validator)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
