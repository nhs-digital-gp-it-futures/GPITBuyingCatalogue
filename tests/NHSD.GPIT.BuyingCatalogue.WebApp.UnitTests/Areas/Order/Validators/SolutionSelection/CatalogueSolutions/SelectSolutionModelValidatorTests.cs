using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.CatalogueSolutions
{
    public static class SelectSolutionModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockAutoData]
        public static void Validate_ValuesProvided_NoValidationErrors(
            SelectSolutionModel model,
            SelectSolutionModelValidator systemUnderTest)
        {
            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
