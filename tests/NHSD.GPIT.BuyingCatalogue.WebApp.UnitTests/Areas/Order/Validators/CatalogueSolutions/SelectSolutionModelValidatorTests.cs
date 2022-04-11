using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.CatalogueSolutions
{
    public static class SelectSolutionModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            string solutionId,
            SelectSolutionModel model,
            SelectSolutionModelValidator systemUnderTest)
        {
            model.SelectedCatalogueSolutionId = solutionId;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SelectedCatalogueSolutionId)
                .WithErrorMessage(SelectSolutionModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValuesProvided_NoValidationErrors(
            SelectSolutionModel model,
            SelectSolutionModelValidator systemUnderTest)
        {
            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
