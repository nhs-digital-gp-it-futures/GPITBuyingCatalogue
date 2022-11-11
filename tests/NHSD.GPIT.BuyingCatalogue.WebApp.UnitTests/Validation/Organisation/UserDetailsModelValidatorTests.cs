using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Organisation
{
    public static class UserDetailsModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.FirstName = firstName;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage("Enter a first name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.LastName = lastName;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage("Enter a last name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_EmailAddressNullOrEmpty_SetsModelError(
            string emailAddress,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage("Enter an email address");
        }

        [Theory]
        [CommonInlineAutoData("test")]
        public static void Validate_EmailAddressInvalidFormat_SetsModelError(
            string emailAddress,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(model => model.EmailAddress)
                .WithErrorMessage("Enter an email address in the correct format, like name@example.com");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UserWithEmailExists_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            var emailAddress = "test@test.com";

            mockUsersService
                .Setup(x => x.EmailAddressExists(emailAddress, model.UserId))
                .ReturnsAsync(true);

            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
                .WithErrorMessage("A user with this email address is already registered on the Buying Catalogue");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_SetsModelError(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.SelectedAccountType = accountType;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage("Select an account type");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Validate_AccountStatusNullOrEmpty_SetsModelError(
            bool? isActive,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.IsActive = isActive;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage("Select an account status");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            string firstName,
            string lastName,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.EmailAddress = "a@a.com";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
