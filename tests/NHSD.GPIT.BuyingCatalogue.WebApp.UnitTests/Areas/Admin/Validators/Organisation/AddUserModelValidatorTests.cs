using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Organisation
{
    public static class AddUserModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.FirstName = firstName;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage("First Name Required");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.LastName = lastName;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage("Last Name Required");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_TelephoneNumberNullOrEmpty_SetsModelError(
            string telephoneNumber,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.TelephoneNumber = telephoneNumber;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.TelephoneNumber)
                .WithErrorMessage("Telephone Number Required");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_EmailAddressNullOrEmpty_SetsModelError(
            string emailAddress,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage("Email Address Required");
        }

        [Theory]
        [CommonInlineAutoData("test")]
        public static void Validate_EmailAddressInvalidFormat_SetsModelError(
            string emailAddress,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(model => model.EmailAddress)
                .WithErrorMessage("Enter an email address in the correct format, like name@example.com");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UserWithEmailExists_SetsModelError(
            [Frozen] Mock<ICreateBuyerService> createBuyerService,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            var emailAddress = "test@test.com";

            createBuyerService.Setup(cbs => cbs.UserExistsWithEmail(emailAddress))
                .ReturnsAsync(true);

            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(model => model.EmailAddress)
                .WithErrorMessage("A user with this email address is already registered on the Buying Catalogue");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            string firstName,
            string lastName,
            string telephoneNumber,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.TelephoneNumber = telephoneNumber;
            model.EmailAddress = "a@a.com";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
