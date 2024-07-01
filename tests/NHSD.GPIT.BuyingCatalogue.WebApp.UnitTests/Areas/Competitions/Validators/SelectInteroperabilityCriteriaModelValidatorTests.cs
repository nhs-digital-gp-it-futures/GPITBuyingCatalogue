using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SelectInteroperabilityCriteriaModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoSelection_SetsModelError(
        SelectInteroperabilityCriteriaModel model,
        SelectInteroperabilityCriteriaModelValidator validator)
    {
        model.Integrations.ForEach(x => x.Value.ForEach(y => y.Selected = false));

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(SelectInteroperabilityCriteriaModelValidator.InputName)
            .WithErrorMessage(SelectInteroperabilityCriteriaModelValidator.MissingSelectionError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        SelectInteroperabilityCriteriaModel model,
        SelectInteroperabilityCriteriaModelValidator validator)
    {
        model.Integrations.ForEach(x => x.Value.ForEach(y => y.Selected = true));

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
