using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class AddImplementationCriteriaModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_EmptyRequirements_SetsModelError(
        AddImplementationCriteriaModel model,
        AddImplementationCriteriaModelValidator validator)
    {
        model.Requirements = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Requirements)
            .WithErrorMessage(AddImplementationCriteriaModelValidator.EmptyRequirementsError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        string requirements,
        AddImplementationCriteriaModel model,
        AddImplementationCriteriaModelValidator validator)
    {
        model.Requirements = requirements;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
