using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Frameworks;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Frameworks;

public static class AddFrameworkModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoFundingType_SetsModelError(
        AddEditFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.FundingTypes.ForEach(x => x.Selected = false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("FundingTypes[0].Selected")
            .WithErrorMessage(AddFrameworkModelValidator.FundingTypeError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NoName_SetsModelError(
        AddEditFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.Name = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(AddFrameworkModelValidator.NameMissingError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_DuplicateName_SetsModelError(
        AddEditFrameworkModel model,
        [Frozen] IFrameworkService frameworkService,
        AddFrameworkModelValidator validator)
    {
        frameworkService
            .FrameworkNameExistsExcludeSelf(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(AddFrameworkModelValidator.NameDuplicationError);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    [MockInlineAutoData("  ")]
    public static void Validate_NoMaximumTerm_SetsModelError(
        string maximumTerm,
        AddEditFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.MaximumTerm = maximumTerm;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximumTerm)
            .WithErrorMessage(AddFrameworkModelValidator.MaximumDurationMissingError);
    }

    [Theory]
    [MockInlineAutoData("1.2")]
    [MockInlineAutoData("test")]
    public static void Validate_NotInteger_SetsModelError(
        string maximumTerm,
        AddEditFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.MaximumTerm = maximumTerm;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximumTerm)
            .WithErrorMessage(AddFrameworkModelValidator.MaximumDurationMustBeANumberError);
    }

    [Theory]
    [MockInlineAutoData("0")]
    [MockInlineAutoData("-5")]
    public static void Validate_GreaterThanZero_SetsModelError(
        string maximumTerm,
        AddEditFrameworkModel model,
        AddFrameworkModelValidator validator)
    {
        model.MaximumTerm = maximumTerm;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximumTerm)
            .WithErrorMessage(AddFrameworkModelValidator.MaximumDurationGreaterThanZeroError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoErrors(
        AddEditFrameworkModel model,
        [Frozen] IFrameworkService frameworkService,
        AddFrameworkModelValidator validator)
    {
        frameworkService
            .FrameworkNameExists(Arg.Any<string>())
            .Returns(false);

        model.Name = new string('a', 9);
        model.FundingTypes.First().Selected = true;
        model.MaximumTerm = "32";

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
