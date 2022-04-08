using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Users
{
    public class PersonalDetailsModelValidatorTests
    {
        private const string EmailAddress = "a@b.com";
        private const int UserId = 1;

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            PersonalDetailsModelValidator validator)
        {
            var model = new PersonalDetailsModel
            {
                FirstName = firstName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(PersonalDetailsModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            PersonalDetailsModelValidator validator)
        {
            var model = new PersonalDetailsModel
            {
                LastName = lastName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage(PersonalDetailsModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_EmailNullOrEmpty_SetsModelError(
            string email,
            PersonalDetailsModelValidator validator)
        {
            var model = new PersonalDetailsModel
            {
                Email = email,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(PersonalDetailsModelValidator.EmailMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailWrongFormat_SetsModelError(
            PersonalDetailsModel model,
            PersonalDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(PersonalDetailsModelValidator.EmailWrongFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailAlreadyExists_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            PersonalDetailsModel model,
            PersonalDetailsModelValidator validator)
        {
            model.Email = EmailAddress;
            model.UserId = UserId;

            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, UserId))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(PersonalDetailsModelValidator.EmailInUseErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EverythingOk_NoModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            PersonalDetailsModel model,
            PersonalDetailsModelValidator validator)
        {
            model.Email = EmailAddress;
            model.UserId = UserId;

            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, UserId))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
