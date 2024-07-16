using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SolutionJustificationModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_JustificationEmpty_SetsModelError(
        SolutionJustificationModel model,
        SolutionJustificationModelValidator validator)
    {
        model.Justification = string.Empty;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Justification)
            .WithErrorMessage(SolutionJustificationModelValidator.JustificationMissingError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        string justification,
        SolutionJustificationModel model,
        SolutionJustificationModelValidator validator)
    {
        model.Justification = justification;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
