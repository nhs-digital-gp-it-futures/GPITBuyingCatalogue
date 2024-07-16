using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderingParty;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class OrderingPartyModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_FirstNameEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.FirstName = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.FirstName)
                .WithErrorMessage("Enter a first name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_LastNameEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.LastName = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.LastName)
                .WithErrorMessage("Enter a last name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_TelephoneEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.TelephoneNumber = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.TelephoneNumber)
                .WithErrorMessage("Enter a telephone number");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.EmailAddress = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.EmailAddress)
                .WithErrorMessage("Enter an email address");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailNotValid_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.EmailAddress = "test";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.EmailAddress)
                .WithErrorMessage("Enter an email address in the correct format, like name@example.com");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoValidationError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.EmailAddress = "a@b.com";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
