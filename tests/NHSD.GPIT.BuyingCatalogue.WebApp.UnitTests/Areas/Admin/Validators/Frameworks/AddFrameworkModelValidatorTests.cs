using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Frameworks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Frameworks;

public static class AddFrameworkModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NoFundingType_SetsModelError(
        AddFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.IsLocalFundingOnly = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsLocalFundingOnly)
            .WithErrorMessage(AddFrameworkModelValidator.FundingTypeError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoName_SetsModelError(
        AddFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.Name = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(AddFrameworkModelValidator.NameMissingError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_DuplicateName_SetsModelError(
        AddFrameworkModel model,
        [Frozen] Mock<IFrameworkService> frameworkService,
        AddFrameworkModelValidator validator)
    {
        frameworkService
            .Setup(x => x.FrameworkNameExists(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(AddFrameworkModelValidator.NameDuplicationError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoErrors(
        AddFrameworkModel model,
        [Frozen] Mock<IFrameworkService> frameworkService,
        AddFrameworkModelValidator validator)
    {
        frameworkService
            .Setup(x => x.FrameworkNameExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        model.Name = new string('a', 9);
        model.IsLocalFundingOnly = true;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
