using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.EmailDomainManagement;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.EmailDomainManagement;

public static class AddEmailDomainModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_Empty_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = string.Empty;

        service.Exists(Arg.Any<string>()).Returns(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Incorrect_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "nhs.net";

        service.Exists(Arg.Any<string>()).Returns(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_TooManyWildcards_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@*.*.nhs.net";

        service.Exists(Arg.Any<string>()).Returns(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.TooManyWildcards);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Duplicate_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@nhs.net";

        service.Exists(Arg.Any<string>()).Returns(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.DuplicateEmailDomain);
    }

    [Theory]
    [MockInlineAutoData("@*")]
    [MockInlineAutoData("@*/nhs.net")]
    public static void Validate_InvalidFormat_SetsModelError(
        string emailDomain,
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = emailDomain;

        service.Exists(Arg.Any<string>()).Returns(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddEmailDomainModel model,
        [Frozen] IEmailDomainService service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@nhs.net";

        service.Exists(Arg.Any<string>()).Returns(false);

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
