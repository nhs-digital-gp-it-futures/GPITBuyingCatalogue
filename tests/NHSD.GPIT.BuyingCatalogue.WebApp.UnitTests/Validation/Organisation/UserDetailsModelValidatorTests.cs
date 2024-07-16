using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockInlineAutoData("test")]
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
        [MockAutoData]
        public static void Validate_InvalidEmailDomain_SetsModelError(
            [Frozen] IEmailDomainService mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.EmailAddress = InvalidEmailAddress;

            mockEmailDomainService.IsAllowed(InvalidEmailAddress).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage("This email domain cannot be used to register a new user account as it is not on the allow list");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_UserWithEmailExists_SetsModelError(
            [Frozen] IUsersService mockUsersService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            mockUsersService.EmailAddressExists(EmailAddress, model.UserId).Returns(true);

            model.EmailAddress = EmailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
                .WithErrorMessage("A user with this email address is already registered on the Buying Catalogue");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_IsNotDefaultType_SetsModelError(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.OrganisationId = 500;
            model.SelectedAccountType = accountType;
            model.IsDefaultAccountType = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage("Select an account type");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_IsDefaultType_NoModelErrors(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.OrganisationId = 500;
            model.SelectedAccountType = accountType;
            model.IsDefaultAccountType = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_AccountTypeNullOrEmpty_NhsDigitalOrganisation_NoModelErrors(
            string accountType,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.OrganisationId = OrganisationConstants.NhsDigitalOrganisationId;
            model.SelectedAccountType = accountType;
            model.IsDefaultAccountType = false;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [MockInlineAutoData(null)]
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
        [MockInlineAutoData(false, "AccountManager", null)]
        [MockInlineAutoData(false, "AccountManager", true)]
        [MockInlineAutoData(false, "AccountManager", false)]
        [MockInlineAutoData(false, "Buyer", null)]
        [MockInlineAutoData(false, "Buyer", true)]
        [MockInlineAutoData(false, "Buyer", false)]
        [MockInlineAutoData(true, "AccountManager", null)]
        [MockInlineAutoData(true, "AccountManager", false)]
        [MockInlineAutoData(true, "Buyer", null)]
        [MockInlineAutoData(true, "Buyer", true)]
        [MockInlineAutoData(true, "Buyer", false)]
        public static void Validate_AccountType_MaxLimit_NoModelError(
            bool isAccountManagerLimit,
            string accountType,
            bool? isActive,
            int organisationId,
            int userId,
            [Frozen] IUsersService mockUsersService,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                OrganisationId = organisationId,
                UserId = userId,
                SelectedAccountType = accountType,
                IsActive = isActive,
            };

            mockUsersService.IsAccountManagerLimit(organisationId, userId).Returns(isAccountManagerLimit);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ActiveAccountManager_MaxLimitTrue_ModelError(
            int organisationId,
            int userId,
            [Frozen] IUsersService mockUsersService,
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

            mockUsersService.IsAccountManagerLimit(organisationId, userId).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage($"There are already {accountManagementSettings.MaximumNumberOfAccountManagers} active account managers for this organisation which is the maximum allowed");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            string firstName,
            string lastName,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IEmailDomainService mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.EmailAddress = "a@nhs.net";

            mockUsersService.EmailAddressExists("a@nhs.net", model.UserId).Returns(false);

            mockEmailDomainService.IsAllowed("a@nhs.net").Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
