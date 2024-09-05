using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.DataProcessingInformation;

public static class AddEditDataProtectionOfficerModelValidatorTests
{
    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static void Validate_EmptyName_SetsModelError(
        string name,
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.Name = name;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(AddEditDataProtectionOfficerModelValidator.NameError);
    }

    [Theory]
    [MockInlineAutoData(null, null)]
    [MockInlineAutoData(null, "")]
    [MockInlineAutoData("", null)]
    [MockInlineAutoData("", "")]
    public static void Validate_MissingEmailAndPhoneNumber_SetsModelError(
        string emailAddress,
        string phoneNumber,
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.EmailAddress = emailAddress;
        model.PhoneNumber = phoneNumber;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
            .WithErrorMessage(AddEditDataProtectionOfficerModelValidator.EmailAndPhoneEmptyError);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage(AddEditDataProtectionOfficerModelValidator.EmailAndPhoneEmptyError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_InvalidEmail_SetsModelError(
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.EmailAddress = "test";
        model.PhoneNumber = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
            .WithErrorMessage(AddEditDataProtectionOfficerModelValidator.InvalidEmailFormatError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_ValidEmail_NoModelErrors(
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.EmailAddress = "test@test.com";
        model.PhoneNumber = null;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.EmailAddress);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_ValidPhoneNumber_NoModelErrors(
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.EmailAddress = null;
        model.PhoneNumber = "123456";

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.EmailAddress);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddEditDataProtectionOfficerModel model,
        AddEditDataProtectionOfficerModelValidator validator)
    {
        model.Name = "name";
        model.EmailAddress = "test@test.com";
        model.PhoneNumber = "123456";

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
