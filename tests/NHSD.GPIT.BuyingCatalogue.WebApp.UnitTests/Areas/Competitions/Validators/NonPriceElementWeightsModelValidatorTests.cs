using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class NonPriceElementWeightsModelValidatorTests
{
    private const int NumberNotDivisibleByFive = 3;

    [Theory]
    [CommonAutoData]
    public static void Validate_HasImplementationWithNullImplementationScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = true;
        model.Implementation = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Implementation)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.ImplementationNullError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoImplementationWithNullImplementationScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = false;
        model.Implementation = null;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Implementation);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_HasImplementationWithInvalidImplementationScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = true;
        model.Implementation = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Implementation)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.ImplementationDivisionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoImplementationWithInvalidImplementationScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = false;
        model.Implementation = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Implementation);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_HasInteroperabilityWithNullInteroperabilityScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasInteroperability = true;
        model.Interoperability = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Interoperability)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.InteroperabilityNullError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoInteroperabilityWithNullInteroperabilityScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasInteroperability = false;
        model.Interoperability = null;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Interoperability);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_HasInteroperabilityWithInvalidInteroperabilityScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasInteroperability = true;
        model.Interoperability = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Interoperability)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.InteroperabilityDivisionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoInteroperabilityWithInvalidInteroperabilityScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasInteroperability = false;
        model.Interoperability = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Interoperability);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_HasServiceLevelWithNullServiceLevelScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasServiceLevel = true;
        model.ServiceLevel = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ServiceLevel)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.ServiceLevelNullError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoServiceLevelWithNullServiceLevelScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasServiceLevel = false;
        model.ServiceLevel = null;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.ServiceLevel);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_HasServiceLevelWithInvalidServiceLevelScore_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasServiceLevel = true;
        model.ServiceLevel = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ServiceLevel)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.ServiceLevelDivisionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoServiceLevelWithInvalidServiceLevelScore_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasServiceLevel = false;
        model.ServiceLevel = NumberNotDivisibleByFive;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.ServiceLevel);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_TotalNotEqualToHundred_SetsModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = model.HasInteroperability = model.HasServiceLevel = true;
        model.Implementation = model.Interoperability = model.ServiceLevel = 10;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Interoperability)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.TotalsInvalidError);
        result.ShouldHaveValidationErrorFor(x => x.Implementation)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.TotalsInvalidError);
        result.ShouldHaveValidationErrorFor(x => x.ServiceLevel)
            .WithErrorMessage(NonPriceElementWeightsModelValidator.TotalsInvalidError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_TotalsEquateToHundred_NoModelError(
        NonPriceElementWeightsModel model,
        NonPriceElementWeightsModelValidator validator)
    {
        model.HasImplementation = model.HasInteroperability = model.HasServiceLevel = true;
        model.Implementation = 50;
        model.Interoperability = model.ServiceLevel = 25;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
