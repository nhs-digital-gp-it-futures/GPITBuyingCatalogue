using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Supplier
{
    public static class NewContactModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_PersonalDetailsMissing_ThrowsValidationError(
            NewContactModel model,
            NewContactModelValidator systemUnderTest)
        {
            model.FirstName = null;
            model.LastName = null;
            model.Department = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage(ContactModelValidator.PersonalDetailsMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_FirstNameMissing_ThrowsValidationError(
            NewContactModel model,
            NewContactModelValidator systemUnderTest)
        {
            model.FirstName = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage(ContactModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_LastNameMissing_ThrowsValidationError(
            NewContactModel model,
            NewContactModelValidator systemUnderTest)
        {
            model.LastName = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage(ContactModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ContactDetailsMissing_ThrowsValidationError(
            NewContactModel model,
            NewContactModelValidator systemUnderTest)
        {
            model.Email = null;
            model.PhoneNumber = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                .WithErrorMessage(ContactModelValidator.ContactDetailsMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailWrongFormat_ThrowsValidationError(
            string email,
            NewContactModel model,
            NewContactModelValidator systemUnderTest)
        {
            model.Email = email;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(ContactModelValidator.EmailAddressFormatErrorMessage);
        }
    }
}
