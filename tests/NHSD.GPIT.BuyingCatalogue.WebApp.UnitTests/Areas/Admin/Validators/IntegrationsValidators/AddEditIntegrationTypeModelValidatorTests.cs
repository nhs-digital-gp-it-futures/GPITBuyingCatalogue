using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.IntegrationsValidators;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.IntegrationsValidators;

public static class AddEditIntegrationTypeModelValidatorTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(AddEditIntegrationTypeModelValidator).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_MissingIntegrationTypeName_SetsModelError(
        AddEditIntegrationTypeModel model,
        AddEditIntegrationTypeModelValidator validator)
    {
        model.IntegrationTypeName = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IntegrationTypeName)
            .WithErrorMessage(AddEditIntegrationTypeModelValidator.MissingIntegrationTypeNameError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_MissingDescription_WhenRequired_SetsModelError(
        AddEditIntegrationTypeModel model,
        AddEditIntegrationTypeModelValidator validator)
    {
        model.RequiresDescription = true;
        model.Description = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(AddEditIntegrationTypeModelValidator.MissingDescriptionError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_MissingDescription_WhenNotRequired_NoModelError(
        AddEditIntegrationTypeModel model,
        AddEditIntegrationTypeModelValidator validator)
    {
        model.RequiresDescription = false;
        model.Description = null;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_DuplicateIntegrationTypeName_SetsModelError(
        AddEditIntegrationTypeModel model,
        [Frozen] IIntegrationsService integrationsService,
        AddEditIntegrationTypeModelValidator validator)
    {
        integrationsService.IntegrationTypeExists(
                model.IntegrationId.GetValueOrDefault(),
                model.IntegrationTypeName,
                Arg.Any<int?>())
            .Returns(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IntegrationTypeName)
            .WithErrorMessage(AddEditIntegrationTypeModelValidator.ExistingIntegrationTypeError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelError(
        AddEditIntegrationTypeModel model,
        [Frozen] IIntegrationsService integrationsService,
        AddEditIntegrationTypeModelValidator validator)
    {
        integrationsService.IntegrationTypeExists(
                model.IntegrationId.GetValueOrDefault(),
                model.IntegrationTypeName,
                Arg.Any<int?>())
            .Returns(false);

        model.RequiresDescription = false;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
