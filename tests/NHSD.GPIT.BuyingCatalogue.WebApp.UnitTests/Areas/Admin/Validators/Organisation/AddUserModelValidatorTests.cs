using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
                .WithErrorMessage(AddUserModelValidator.FirstNameError);
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
                .WithErrorMessage(AddUserModelValidator.LastNameError);
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
                .WithErrorMessage(AddUserModelValidator.NoEmailError);
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
                .WithErrorMessage(AddUserModelValidator.EmailFormatError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UserWithEmailExists_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            var emailAddress = "test@test.com";

            mockUsersService
                .Setup(x => x.EmailAddressExists(emailAddress, 0))
                .ReturnsAsync(true);

            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(model => model.EmailAddress)
                .WithErrorMessage(AddUserModelValidator.UserAlreadyExistsError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            string firstName,
            string lastName,
            AddUserModel model,
            AddUserModelValidator validator)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.EmailAddress = "a@a.com";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
