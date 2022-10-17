using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Users
{
    public class AccountTypeModelValidatorTests
    {
        private const int UserId = 1;

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_AccountTypeNullOrEmpty_SetsModelError(
            string accountType,
            AccountTypeModelValidator validator)
        {
            var model = new AccountTypeModel
            {
                SelectedAccountType = accountType,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(AccountTypeModelValidator.SelectedAccountTypeEmptyErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsBuyer_NoModelError(
            AccountTypeModelValidator validator)
        {
            var model = new AccountTypeModel
            {
                SelectedAccountType = OrganisationFunction.Buyer.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_UserInNhsDigital_NoModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            AccountTypeModelValidator validator)
        {
            var model = new AccountTypeModel
            {
                SelectedAccountType = OrganisationFunction.Authority.Name,
                UserId = UserId,
            };

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    PrimaryOrganisationId = OrganisationConstants.NhsDigitalOrganisationId,
                });

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedAccountType);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AccountTypeIsAdmin_UserNotInNhsDigital_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            AccountTypeModelValidator validator)
        {
            var model = new AccountTypeModel
            {
                SelectedAccountType = OrganisationFunction.Authority.Name,
                UserId = UserId,
            };

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    PrimaryOrganisationId = OrganisationConstants.NhsDigitalOrganisationId + 1,
                });

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedAccountType)
                .WithErrorMessage(AccountTypeModelValidator.MustBelongToNhsDigitalErrorMessage);
        }
    }
}
