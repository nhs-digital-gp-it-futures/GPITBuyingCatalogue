using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DataProcessing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts;

public class DataProcessingPlanModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_UseDefaultDataProcessingNull_SetsModelError(
        DataProcessingPlanModel model,
        DataProcessingPlanModelValidator validator)
    {
        model.UseDefaultDataProcessing = null;

        var result = validator
            .TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.UseDefaultDataProcessing)
            .WithErrorMessage(DataProcessingPlanModelValidator.DefaultDataProcessingNullErrorMessage);
    }

    [Theory]
    [CommonInlineAutoData(true)]
    [CommonInlineAutoData(false)]
    public static void Validate_Valid_NoModelError(
        bool useDefaultDataProcessing,
        DataProcessingPlanModel model,
        DataProcessingPlanModelValidator validator)
    {
        model.UseDefaultDataProcessing = useDefaultDataProcessing;

        var result = validator
            .TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
