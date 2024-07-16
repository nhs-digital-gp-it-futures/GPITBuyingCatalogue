using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Validators.Filters
{
    public static class FilterCapabilitiesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            FilterCapabilitiesModel model,
            FilterCapabilitiesModelValidator validator)
        {
            model.CapabilitySelectionItems.ForEach(x => x.Selected = false);
            model.IsFilter = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(FilterCapabilitiesModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_NoErrors(
            FilterCapabilitiesModel model,
            FilterCapabilitiesModelValidator validator)
        {
            model.CapabilitySelectionItems.ForEach(x => x.Selected = false);
            model.IsFilter = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_SelectionMade_NoErrors(
            bool isFiltering,
            FilterCapabilitiesModel model,
            FilterCapabilitiesModelValidator validator)
        {
            model.CapabilitySelectionItems.ForEach(x => x.Selected = true);
            model.IsFilter = isFiltering;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
