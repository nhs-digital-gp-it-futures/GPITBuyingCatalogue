using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class CompetitionWeightingsModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NullPriceWeight_SetsModelError(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.Price = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage(CompetitionWeightingsModelValidator.PriceWeightingNullError);
    }

    [Theory]
    [MockInlineAutoData(25)]
    [MockInlineAutoData(95)]
    public static void Validate_PriceWeightRangeInvalid_SetsModelError(
        int priceWeight,
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.Price = priceWeight;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage(CompetitionWeightingsModelValidator.PriceWeightingRangeError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_PriceWeightNotDivisibleBy5_SetsModelError(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.Price = 33;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage(CompetitionWeightingsModelValidator.PriceWeightingMultiplesError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NullNonPriceWeight_SetsModelError(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.NonPrice = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NonPrice)
            .WithErrorMessage(CompetitionWeightingsModelValidator.NonPriceWeightingNullError);
    }

    [Theory]
    [MockInlineAutoData(5)]
    [MockInlineAutoData(75)]
    public static void Validate_NonPriceWeightRangeInvalid_SetsModelError(
        int priceWeight,
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.NonPrice = priceWeight;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NonPrice)
            .WithErrorMessage(CompetitionWeightingsModelValidator.NonPriceWeightingRangeError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NonPriceWeightNotDivisibleBy5_SetsModelError(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.NonPrice = 13;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NonPrice)
            .WithErrorMessage(CompetitionWeightingsModelValidator.NonPriceWeightingMultiplesError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_WeightingsTotalsInvalid_SetsModelError(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.Price = model.NonPrice = 70;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage(CompetitionWeightingsModelValidator.TotalsInvalidError);

        result.ShouldHaveValidationErrorFor(x => x.NonPrice)
            .WithErrorMessage(CompetitionWeightingsModelValidator.TotalsInvalidError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        CompetitionWeightingsModel model,
        CompetitionWeightingsModelValidator validator)
    {
        model.Price = model.NonPrice = 50;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
