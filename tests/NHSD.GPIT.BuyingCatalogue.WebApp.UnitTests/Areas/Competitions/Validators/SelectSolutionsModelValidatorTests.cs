using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SelectSolutionsModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_SingleSolutionMissingAwardSelection_SetsModelError(
        SolutionModel solution,
        SelectSolutionsModel model,
        SelectSolutionsModelValidator validator)
    {
        model.IsDirectAward = null;
        model.Solutions = new() { solution };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsDirectAward)
            .WithErrorMessage(SelectSolutionsModelValidator.DirectAwardSelectionMissingError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_SingleSolutionValid_NoModelError(
        SolutionModel solution,
        SelectSolutionsModel model,
        SelectSolutionsModelValidator validator)
    {
        model.IsDirectAward = true;
        model.Solutions = new() { solution };

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_MultipleSolutionsNotEnoughSelections_SetsModelError(
        List<SolutionModel> solutions,
        SelectSolutionsModel model,
        SelectSolutionsModelValidator validator)
    {
        solutions.Take(1).ToList().ForEach(x => x.Selected = true);
        solutions.Skip(1).ToList().ForEach(x => x.Selected = false);

        model.Solutions = solutions;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(SelectSolutionsModelValidator.SelectedSolutionsPropertyName)
            .WithErrorMessage(SelectSolutionsModelValidator.NotEnoughSelectionsError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_MultipleSolutionsTooManySelections_SetsModelError(
        IFixture fixture,
        SelectSolutionsModel model,
        SelectSolutionsModelValidator validator)
    {
        var solutions = fixture.CreateMany<SolutionModel>(10).ToList();

        solutions.ForEach(x => x.Selected = true);

        model.Solutions = solutions;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(SelectSolutionsModelValidator.SelectedSolutionsPropertyName)
            .WithErrorMessage(SelectSolutionsModelValidator.TooManySelectionsError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_MultipleSolutionsValid_NoModelErrors(
        IFixture fixture,
        SelectSolutionsModel model,
        SelectSolutionsModelValidator validator)
    {
        var solutions = fixture.CreateMany<SolutionModel>(10).ToList();

        solutions.ForEach(x => x.Selected = false);
        solutions.Take(3).ToList().ForEach(x => x.Selected = true);

        model.Solutions = solutions;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
