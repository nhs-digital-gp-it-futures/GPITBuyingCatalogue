using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Organisation
{
    public static class UserDetailsModelValidatorTests
    {
        private const string InvalidEmailAddress = "a@b.com";
        private const string EmailAddress = "a@nhs.net";

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
        [CommonInlineAutoData(" ")]
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
        public static void Validate_InvalidEmailDomain_SetsModelError(
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.EmailAddress = InvalidEmailAddress;

            mockEmailDomainService.Setup(s => s.IsAllowed(InvalidEmailAddress))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage("This email domain cannot be used to register a new user account as it is not on the allow list");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UserWithEmailExists_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, model.UserId))
                .ReturnsAsync(true);

            model.EmailAddress = EmailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
                .WithErrorMessage("A user with this email address is already registered on the Buying Catalogue");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_IsNotDefaultType_SetsModelError(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.SelectedAccountType = accountType;
            model.IsDefaultAccountType = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage("Select an account type");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_IsDefaultType_NoModelErrors(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.SelectedAccountType = accountType;
            model.IsDefaultAccountType = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
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
        [CommonInlineAutoData(false, "AccountManager", null)]
        [CommonInlineAutoData(false, "AccountManager", true)]
        [CommonInlineAutoData(false, "AccountManager", false)]
        [CommonInlineAutoData(false, "Buyer", null)]
        [CommonInlineAutoData(false, "Buyer", true)]
        [CommonInlineAutoData(false, "Buyer", false)]
        [CommonInlineAutoData(true, "AccountManager", null)]
        [CommonInlineAutoData(true, "AccountManager", false)]
        [CommonInlineAutoData(true, "Buyer", null)]
        [CommonInlineAutoData(true, "Buyer", true)]
        [CommonInlineAutoData(true, "Buyer", false)]
        public static void Validate_AccountType_MaxLimit_NoModelError(
            bool isAccountManagerLimit,
            string accountType,
            bool? isActive,
            int organisationId,
            int userId,
            [Frozen] Mock<IUsersService> mockUsersService,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                OrganisationId = organisationId,
                UserId = userId,
                SelectedAccountType = accountType,
                IsActive = isActive,
            };

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisationId, userId))
                .ReturnsAsync(isAccountManagerLimit);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ActiveAccountManager_MaxLimitTrue_ModelError(
            int organisationId,
            int userId,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] AccountManagementSettings accountManagementSettings,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                OrganisationId = organisationId,
                UserId = userId,
                SelectedAccountType = OrganisationFunction.AccountManager.Name,
                IsActive = true,
            };

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisationId, userId))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage($"There are already {accountManagementSettings.MaximumNumberOfAccountManagers} active account managers for this organisation which is the maximum allowed.");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            string firstName,
            string lastName,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.EmailAddress = "a@nhs.net";

            mockUsersService
                .Setup(x => x.EmailAddressExists("a@nhs.net", model.UserId))
                .ReturnsAsync(false);

            mockEmailDomainService
                .Setup(x => x.IsAllowed("a@nhs.net"))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
