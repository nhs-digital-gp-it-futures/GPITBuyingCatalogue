using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_SelectedOrganisationIdNullOrEmpty_SetsModelError(
            string organisationId,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = organisationId,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(UserDetailsModelValidator.OrganisationMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonAutoData]
        public static void Validate_EmailInvalidFormat_SetsModelError(
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailWrongFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailInvalidDomain_SetsModelError(
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = InvalidEmailAddress;

            mockEmailDomainService.Setup(s => s.IsAllowed(InvalidEmailAddress))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailDomainInvalid);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailInUse_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = EmailAddress;

            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, model.UserId))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(UserDetailsModelValidator.EmailInUseErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNotNhsDigital_SetsModelError(
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId + 1}",
                SelectedAccountType = OrganisationFunction.Authority.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(UserDetailsModelValidator.MustBelongToNhsDigitalErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNhsDigital_NoModelError(
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId}",
                SelectedAccountType = OrganisationFunction.Authority.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAccountManager_NoModelError(
            int organisationId,
            [Frozen] Mock<IUsersService> mockUsersService,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = $"{organisationId}",
                SelectedAccountType = OrganisationFunction.AccountManager.Name,
            };

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisationId, 0))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAccountManager_ModelError(
            int organisationId,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] AccountManagementSettings accountManagementSettings,
            UserDetailsModelValidator validator)
        {
            var model = new UserDetailsModel
            {
                SelectedOrganisationId = $"{organisationId}",
                SelectedAccountType = OrganisationFunction.AccountManager.Name,
            };

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisationId, 0))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(string.Format(UserDetailsModelValidator.MustNotExceedAccountManagerLimit, accountManagementSettings.MaximumNumberOfAccountManagers));
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
                .WithErrorMessage(UserDetailsModelValidator.AccountStatusMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EverythingOk_NoModelErrors(
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            UserDetailsModel model,
            UserDetailsModelValidator validator)
        {
            model.Email = EmailAddress;
            model.SelectedAccountType = OrganisationFunction.Buyer.Name;

            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, 0))
                .ReturnsAsync(false);

            mockEmailDomainService
                .Setup(x => x.IsAllowed(EmailAddress))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
