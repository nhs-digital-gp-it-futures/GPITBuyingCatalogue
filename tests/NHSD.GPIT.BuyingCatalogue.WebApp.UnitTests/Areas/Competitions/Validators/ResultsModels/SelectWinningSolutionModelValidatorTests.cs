using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.Results;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators.ResultsModels;

public static class SelectWinningSolutionModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_MissingSolutionId_SetsModelError(
        SelectWinningSolutionModel model,
        SelectWinningSolutionModelValidator validator)
    {
        model.SolutionId = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SolutionId)
            .WithErrorMessage(SelectWinningSolutionModelValidator.SolutionNotSelectedError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_ValidSolutionId_NoModelError(
        CatalogueItemId solutionId,
        SelectWinningSolutionModel model,
        SelectWinningSolutionModelValidator validator)
    {
        model.SolutionId = solutionId;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
