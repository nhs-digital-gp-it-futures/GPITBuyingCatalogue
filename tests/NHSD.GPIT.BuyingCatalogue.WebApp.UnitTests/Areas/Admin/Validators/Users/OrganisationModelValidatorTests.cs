using System.Collections.Generic;
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
    public class OrganisationModelValidatorTests
    {
        private const int NotNhsDigitalOrganisationId = 2;
        private const int UserId = 1;

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_OrganisationIdNullOrEmpty_SetsModelError(
            string organisationId,
            OrganisationModelValidator validator)
        {
            var model = new OrganisationModel
            {
                SelectedOrganisationId = organisationId,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(OrganisationModelValidator.OrganisationMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AdminUser_NotInNhsDigital_SetsModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationModel model,
            OrganisationModelValidator validator)
        {
            model.SelectedOrganisationId = $"{NotNhsDigitalOrganisationId}";
            model.UserId = UserId;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(
                    new AspNetUser());

            mockUsersService
                .Setup(x => x.HasRole(UserId, OrganisationFunction.AuthorityName))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(OrganisationModelValidator.MustBelongToNhsDigitalErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AdminUser_InNhsDigital_NoModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationModel model,
            OrganisationModelValidator validator)
        {
            model.SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId}";
            model.UserId = UserId;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    AspNetUserRoles = new List<AspNetUserRole>
                    {
                        new() { Role = new() { Name = OrganisationFunction.AuthorityName } },
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedOrganisationId);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NonAdminUser_NotInNhsDigital_NoModelError(
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationModel model,
            OrganisationModelValidator validator)
        {
            model.SelectedOrganisationId = $"{NotNhsDigitalOrganisationId}";
            model.UserId = UserId;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    AspNetUserRoles = new List<AspNetUserRole>
                    {
                        new() { Role = new() { Name = OrganisationFunction.BuyerName } },
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
