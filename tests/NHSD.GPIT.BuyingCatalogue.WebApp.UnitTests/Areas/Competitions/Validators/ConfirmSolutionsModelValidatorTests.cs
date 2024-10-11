using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators
{
    public static class ConfirmSolutionsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            ConfirmSolutionsModel model,
            ConfirmSolutionsModelValidator systemUnderTest)
        {
            model.ConfirmShortlist = false;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmShortlist)
                .WithErrorMessage(ConfirmSolutionsModelValidator.ConfirmShortlistError);
        }
    }
}
