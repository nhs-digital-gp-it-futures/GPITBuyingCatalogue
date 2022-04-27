using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Solutions
{
    public static class SelectSolutionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedSolutionNull_SetsModelError(
            SelectSolutionModel model,
            SelectSolutionModelValidator validator)
        {
            model.SelectedSolutionId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedSolutionId)
                .WithErrorMessage("Select a Catalogue Solution");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectSolutionModel model,
            SelectSolutionModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
