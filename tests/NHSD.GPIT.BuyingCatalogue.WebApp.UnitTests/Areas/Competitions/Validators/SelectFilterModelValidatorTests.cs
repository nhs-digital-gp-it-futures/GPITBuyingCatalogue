using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SelectFilterModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NoSelection_SetsModelError(
        SelectFilterModel model,
        SelectFilterModelValidator validator)
    {
        model.SelectedFilterId = null;

        var result = validator.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.SelectedFilterId)
            .WithErrorMessage("Select a filter");
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_ValidSelection_NoModelError(
        SelectFilterModel model,
        SelectFilterModelValidator validator)
    {
        model.SelectedFilterId = 1;

        var result = validator.TestValidate(model);

        result
            .ShouldNotHaveAnyValidationErrors();
    }
}
