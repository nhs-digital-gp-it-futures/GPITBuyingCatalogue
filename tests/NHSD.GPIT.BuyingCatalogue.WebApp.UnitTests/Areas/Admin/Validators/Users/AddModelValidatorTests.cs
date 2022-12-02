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
    public class AddModelValidatorTests
    {
        private const string EmailAddress = "a@b.com";

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_SelectedOrganisationIdNullOrEmpty_SetsModelError(
            string organisationId,
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                SelectedOrganisationId = organisationId,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(AddModelValidator.OrganisationMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                FirstName = firstName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(AddModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                LastName = lastName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage(AddModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_EmailNullOrEmpty_SetsModelError(
            string email,
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                Email = email,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(AddModelValidator.EmailMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailWrongFormat_SetsModelError(
            AddModel model,
            AddModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(AddModelValidator.EmailWrongFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidEmailDomain_SetsModelError(
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            AddModel model,
            AddModelValidator validator)
        {
            model.Email = EmailAddress;

            mockEmailDomainService.Setup(s => s.IsAllowed(EmailAddress))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(AddModelValidator.EmailDomainInvalid);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailInUse_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            AddModel model,
            AddModelValidator validator)
        {
            model.Email = EmailAddress;

            mockUsersService
                .Setup(x => x.EmailAddressExists(EmailAddress, 0))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(AddModelValidator.EmailInUseErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_AccountTypeNullOrEmpty_SetsModelError(
            string accountType,
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                SelectedAccountType = accountType,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(AddModelValidator.AccountTypeMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNotNhsDigital_SetsModelError(
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId + 1}",
                SelectedAccountType = OrganisationFunction.Authority.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(AddModelValidator.MustBelongToNhsDigitalErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_OrganisationIdNhsDigital_NoModelError(
            AddModelValidator validator)
        {
            var model = new AddModel
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
            AddModelValidator validator)
        {
            var model = new AddModel
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
            AddModelValidator validator)
        {
            var model = new AddModel
            {
                SelectedOrganisationId = $"{organisationId}",
                SelectedAccountType = OrganisationFunction.AccountManager.Name,
            };

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisationId, 0))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(string.Format(AddModelValidator.MustNotExceedAccountManagerLimit, accountManagementSettings.MaximumNumberOfAccountManagers));
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Validate_AccountStatusNullOrEmpty_SetsModelError(
            bool? isActive,
            AddModel model,
            AddModelValidator validator)
        {
            model.IsActive = isActive;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage(AddModelValidator.AccountStatusMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EverythingOk_NoModelErrors(
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IEmailDomainService> mockEmailDomainService,
            AddModel model,
            AddModelValidator validator)
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
