using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public sealed class CompetitionContractModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_ContractLengthMissing_SetsModelError(
        CompetitionContractModel model,
        CompetitionContractModelValidator validator)
    {
        model.ContractLength = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ContractLength)
            .WithErrorMessage(CompetitionContractModelValidator.ContractLengthMissing);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_ExceedsMaxLimit_SetsModelError(
        CompetitionContractModel model,
        CompetitionContractModelValidator validator)
    {
        model.ContractLength = 37;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ContractLength)
            .WithErrorMessage(CompetitionContractModelValidator.ExceedsLimitError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_ValueTooLow_SetsModelError(
        CompetitionContractModel model,
        CompetitionContractModelValidator validator)
    {
        model.ContractLength = 0;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ContractLength)
            .WithErrorMessage(CompetitionContractModelValidator.TooLowError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        CompetitionContractModel model,
        CompetitionContractModelValidator validator)
    {
        model.ContractLength = 24;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
