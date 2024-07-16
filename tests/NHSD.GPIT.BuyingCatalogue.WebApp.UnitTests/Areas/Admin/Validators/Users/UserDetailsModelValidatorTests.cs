using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Users
{
    public class UserDetailsModelValidatorTests
    {
        private const string InvalidEmailAddress = "a@b.com";
        private const string EmailAddress = "a@nhs.net";

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedOrganisationIdNullOrEmpty_SetsModelError(
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = null,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(UserDetailsModelValidator.OrganisationMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                FirstName = firstName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(UserDetailsModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                LastName = lastName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage(UserDetailsModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_EmailNullOrEmpty_SetsModelError(
            string email,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                Email = email,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailInvalidFormat_SetsModelError(
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailWrongFormatErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailInvalidDomain_SetsModelError(
            [Frozen] IEmailDomainService mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = InvalidEmailAddress;

            mockEmailDomainService.IsAllowed(InvalidEmailAddress).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailDomainInvalid);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailInUse_SetsModelError(
            [Frozen] IUsersService mockUsersService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = EmailAddress;

            mockUsersService.EmailAddressExists(EmailAddress, model.UserId).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailInUseErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_AccountTypeNullOrEmpty_SetsModelError(
            string accountType,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedAccountType = accountType,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(UserDetailsModelValidator.AccountTypeMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNotNhsDigital_SetsModelError(
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = OrganisationConstants.NhsDigitalOrganisationId + 1,
                SelectedAccountType = OrganisationFunction.Authority.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(UserDetailsModelValidator.MustBelongToNhsDigitalErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNhsDigital_NoModelError(
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = OrganisationConstants.NhsDigitalOrganisationId,
                SelectedAccountType = OrganisationFunction.Authority.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [MockInlineAutoData(false, "AccountManager", null)]
        [MockInlineAutoData(false, "AccountManager", true)]
        [MockInlineAutoData(false, "AccountManager", false)]
        [MockInlineAutoData(false, "Buyer", null)]
        [MockInlineAutoData(false, "Buyer", true)]
        [MockInlineAutoData(false, "Buyer", false)]
        [MockInlineAutoData(false, "Authority", null)]
        [MockInlineAutoData(false, "Authority", true)]
        [MockInlineAutoData(false, "Authority", false)]
        [MockInlineAutoData(true, "AccountManager", null)]
        [MockInlineAutoData(true, "AccountManager", false)]
        [MockInlineAutoData(true, "Buyer", null)]
        [MockInlineAutoData(true, "Buyer", true)]
        [MockInlineAutoData(true, "Buyer", false)]
        [MockInlineAutoData(true, "Authority", null)]
        [MockInlineAutoData(true, "Authority", true)]
        [MockInlineAutoData(true, "Authority", false)]
        public static void Validate_AccountType_MaxLimit_NoModelError(
            bool isAccountManagerLimit,
            string accountType,
            bool? isActive,
            int userId,
            [Frozen] IUsersService mockUsersService,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = OrganisationConstants.NhsDigitalOrganisationId,
                UserId = userId,
                SelectedAccountType = accountType,
                IsActive = isActive,
            };

            mockUsersService.IsAccountManagerLimit(OrganisationConstants.NhsDigitalOrganisationId, userId).Returns(isAccountManagerLimit);

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
                SelectedOrganisationId = organisationId,
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
        [MockInlineAutoData(null)]
        public static void Validate_AccountStatusNullOrEmpty_SetsModelError(
            bool? isActive,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.IsActive = isActive;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage(UserDetailsModelValidator.AccountStatusMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EverythingOk_NoModelErrors(
            [Frozen] IUsersService mockUsersService,
            [Frozen] IEmailDomainService mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = EmailAddress;
            model.SelectedAccountType = OrganisationFunction.Buyer.Name;

            mockUsersService.EmailAddressExists(EmailAddress, 0).Returns(false);

            mockEmailDomainService.IsAllowed(EmailAddress).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
