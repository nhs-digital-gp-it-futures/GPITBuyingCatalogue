using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.EmailDomainManagement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.EmailDomainManagement;

public static class AddEmailDomainModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_Empty_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = string.Empty;

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Incorrect_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "nhs.net";

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_TooManyWildcards_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@*.*.nhs.net";

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.TooManyWildcards);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Duplicate_SetsModelError(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@nhs.net";

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.DuplicateEmailDomain);
    }

    [Theory]
    [CommonInlineAutoData("@*")]
    [CommonInlineAutoData("@*/nhs.net")]
    public static void Validate_InvalidFormat_SetsModelError(
        string emailDomain,
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = emailDomain;

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.EmailDomain)
            .WithErrorMessage(AddEmailDomainModelValidator.EmailDomainInvalid);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        AddEmailDomainModelValidator validator)
    {
        model.EmailDomain = "@nhs.net";

        service.Setup(s => s.Exists(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
