using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.NonPriceElements;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators.NonPriceElements;

public static class FeaturesRequirementModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_MissingRequirements_SetsModelError(
        FeaturesRequirementModel model,
        FeaturesRequirementModelValidator validator)
    {
        model.Requirements = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Requirements)
            .WithErrorMessage(FeaturesRequirementModelValidator.MissingRequirementsError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_MissingCompliance_SetsModelError(
        FeaturesRequirementModel model,
        FeaturesRequirementModelValidator validator)
    {
        model.SelectedCompliance = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SelectedCompliance)
            .WithErrorMessage(FeaturesRequirementModelValidator.MissingComplianceError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        string requirements,
        CompliancyLevel selectedCompliance,
        FeaturesRequirementModel model,
        FeaturesRequirementModelValidator validator)
    {
        model.Requirements = requirements;
        model.SelectedCompliance = selectedCompliance;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
