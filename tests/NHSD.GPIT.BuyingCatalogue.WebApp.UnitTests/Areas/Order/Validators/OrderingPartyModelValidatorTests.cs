using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class OrderingPartyModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_FirstNameEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.FirstName = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.FirstName)
                .WithErrorMessage(OrderingPartyModelValidator.FirstNameErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_LastNameEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.LastName = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.LastName)
                .WithErrorMessage(OrderingPartyModelValidator.LastNameErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_TelephoneEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.TelephoneNumber = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.TelephoneNumber)
                .WithErrorMessage(OrderingPartyModelValidator.PhoneNumberErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailEmpty_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.EmailAddress = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.EmailAddress)
                .WithErrorMessage(OrderingPartyModelValidator.NoEmailAddressErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailNotValid_SetsModelError(
            OrderingPartyModel model,
            OrderingPartyModelValidator validator)
        {
            model.Contact.EmailAddress = "test";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Contact.EmailAddress)
                .WithErrorMessage(OrderingPartyModelValidator.InvalidEmailAddressFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
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
