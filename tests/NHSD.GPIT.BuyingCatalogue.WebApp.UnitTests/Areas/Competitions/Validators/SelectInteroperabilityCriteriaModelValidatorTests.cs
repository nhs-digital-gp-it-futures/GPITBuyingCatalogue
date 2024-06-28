using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SelectInteroperabilityCriteriaModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NoSelection_SetsModelError(
        SelectInteroperabilityCriteriaModel model,
        SelectInteroperabilityCriteriaModelValidator validator)
    {
        model.Im1Integrations.ForEach(x => x.Selected = false);
        model.GpConnectIntegrations.ForEach(x => x.Selected = false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(SelectInteroperabilityCriteriaModelValidator.PropertyName)
            .WithErrorMessage(SelectInteroperabilityCriteriaModelValidator.MissingSelectionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        SelectInteroperabilityCriteriaModel model,
        SelectInteroperabilityCriteriaModelValidator validator)
    {
        model.Im1Integrations.Take(1).ToList().ForEach(x => x.Selected = true);
        model.GpConnectIntegrations.Take(1).ToList().ForEach(x => x.Selected = true);

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
