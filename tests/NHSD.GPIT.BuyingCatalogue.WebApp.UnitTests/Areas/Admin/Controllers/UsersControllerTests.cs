using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class UsersControllerTests
    {
        private const int UserId = 1;

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(UsersController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(UsersController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UsersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_GetsAllUsers(
            List<AspNetUser> users,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(users);

            var result = await controller.Index();

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<IndexModel>().Subject;

            model.PageOptions.NumberOfPages.Should().Be(1);
            model.PageOptions.PageNumber.Should().Be(1);
            model.PageOptions.PageSize.Should().Be(UsersController.PageSize);
            model.PageOptions.TotalNumberOfItems.Should().Be(users.Count);
            model.SearchTerm.Should().BeNull();
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_WithSearchTerm_GetsAllMatchingUsers(
            string searchTerm,
            List<AspNetUser> users,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetAllUsersBySearchTerm(searchTerm))
                .ReturnsAsync(users);

            var result = await controller.Index(search: searchTerm);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<IndexModel>().Subject;

            model.PageOptions.NumberOfPages.Should().Be(1);
            model.PageOptions.PageNumber.Should().Be(1);
            model.PageOptions.PageSize.Should().Be(UsersController.PageSize);
            model.PageOptions.TotalNumberOfItems.Should().Be(users.Count);
            model.SearchTerm.Should().Be(searchTerm);
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            List<AspNetUser> users,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(o => o.GetAllUsersBySearchTerm(searchTerm))
                .ReturnsAsync(users);

            var result = await controller.SearchResults(searchTerm);

            mockUsersService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            foreach (var user in users)
            {
                actualResult.Should().Contain(x => x.Title == user.FullName && x.Category == user.Email);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(o => o.GetAllUsersBySearchTerm(searchTerm))
                .ReturnsAsync(new List<AspNetUser>());

            var result = await controller.SearchResults(searchTerm);

            mockUsersService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_SearchResults_InvalidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            UsersController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Add_ReturnsExpectedResult(
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            UsersController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var result = await controller.Add();

            mockOrganisationsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<AddModel>().Subject;

            foreach (var organisation in organisations)
            {
                model.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Add_WithModelErrors_ReturnsExpectedResult(
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            AddModel model,
            UsersController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.Add(model);

            mockOrganisationsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeAssignableTo<AddModel>().Subject;

            foreach (var organisation in organisations)
            {
                actualModel.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Add_ReturnsExpectedResult(
            [Frozen] Mock<ICreateUserService> mockCreateUserService,
            AddModel model,
            UsersController controller)
        {
            model.SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId}";

            mockCreateUserService
                .Setup(x => x.Create(
                    OrganisationConstants.NhsDigitalOrganisationId,
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.SelectedAccountType,
                    model.SelectedAccountStatus))
                .ReturnsAsync(new AspNetUser());

            var result = await controller.Add(model);

            mockCreateUserService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Details_ReturnsExpectedResult(
            AspNetUser user,
            List<EntityFramework.Ordering.Models.Order> orders,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrderService> mockOrderService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            mockOrderService
                .Setup(x => x.GetUserOrders(UserId))
                .ReturnsAsync(orders);

            var result = await controller.Details(UserId);

            mockUsersService.VerifyAll();
            mockOrderService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<DetailsModel>().Subject;

            model.User.Should().Be(user);
            model.Orders.Should().BeEquivalentTo(orders);
        }

        [Theory]
        [CommonInlineAutoData(true, AccountStatus.Inactive)]
        [CommonInlineAutoData(false, AccountStatus.Active)]
        public static async Task Get_AccountStatus_ReturnsExpectedResult(
            bool disabled,
            AccountStatus accountStatus,
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            user.Disabled = disabled;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            var result = await controller.AccountStatus(UserId);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<AccountStatusModel>().Subject;

            model.Email.Should().Be(user.Email);
            model.SelectedAccountStatusId.Should().Be($"{accountStatus}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AccountStatus_WithModelErrors_ReturnsExpectedResult(
            AccountStatusModel model,
            UsersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.AccountStatus(UserId, model);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeOfType<AccountStatusModel>().Subject;

            actualModel.Should().Be(model);
        }

        [Theory]
        [CommonInlineAutoData(true, AccountStatus.Inactive)]
        [CommonInlineAutoData(false, AccountStatus.Active)]
        public static async Task Post_AccountStatus_ReturnsExpectedResult(
            bool disabled,
            AccountStatus accountStatus,
            AccountStatusModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            model.SelectedAccountStatusId = $"{accountStatus}";

            mockUsersService
                .Setup(x => x.EnableOrDisableUser(UserId, disabled))
                .Returns(Task.CompletedTask);

            var result = await controller.AccountStatus(UserId, model);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Details));
            actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "userId", UserId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AccountType_ReturnsExpectedResult(
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            var result = await controller.AccountType(UserId);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<AccountTypeModel>().Subject;

            model.Email.Should().Be(user.Email);
            model.SelectedAccountType.Should().Be(user.OrganisationFunction);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AccountType_WithModelErrors_ReturnsExpectedResult(
            AccountTypeModel model,
            UsersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.AccountType(UserId, model);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeOfType<AccountTypeModel>().Subject;

            actualModel.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AccountType_ReturnsExpectedResult(
            AccountTypeModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.UpdateUserAccountType(UserId, model.SelectedAccountType))
                .Returns(Task.CompletedTask);

            var result = await controller.AccountType(UserId, model);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Details));
            actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "userId", UserId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Organisation_ReturnsExpectedResult(
            AspNetUser user,
            List<Organisation> organisations,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var result = await controller.Organisation(UserId);

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<OrganisationModel>().Subject;

            model.Email.Should().Be(user.Email);

            foreach (var organisation in organisations)
            {
                model.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Organisation_WithModelErrors_ReturnsExpectedResult(
            OrganisationModel model,
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            UsersController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.Organisation(UserId, model);

            mockOrganisationsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeOfType<OrganisationModel>().Subject;

            foreach (var organisation in organisations)
            {
                actualModel.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Organisation_ReturnsExpectedResult(
            OrganisationModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            model.SelectedOrganisationId = $"{OrganisationConstants.NhsDigitalOrganisationId}";

            mockUsersService
                .Setup(x => x.UpdateUserOrganisation(UserId, OrganisationConstants.NhsDigitalOrganisationId))
                .Returns(Task.CompletedTask);

            var result = await controller.Organisation(UserId, model);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Details));
            actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "userId", UserId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PersonalDetails_ReturnsExpectedResult(
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            var result = await controller.PersonalDetails(UserId);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<PersonalDetailsModel>().Subject;

            model.FirstName.Should().Be(user.FirstName);
            model.LastName.Should().Be(user.LastName);
            model.Email.Should().Be(user.Email);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PersonalDetails_WithModelErrors_ReturnsExpectedResult(
            PersonalDetailsModel model,
            UsersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.PersonalDetails(UserId, model);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeOfType<PersonalDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PersonalDetails_ReturnsExpectedResult(
            PersonalDetailsModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.UpdateUserDetails(UserId, model.FirstName, model.LastName, model.Email))
                .Returns(Task.CompletedTask);

            var result = await controller.PersonalDetails(UserId, model);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Details));
            actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "userId", UserId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ResetPassword_ReturnsExpectedResult(
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            var result = await controller.ResetPassword(UserId);

            mockUsersService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeOfType<ResetPasswordModel>().Subject;

            model.Email.Should().Be(user.Email);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ResetPassword_ReturnsExpectedResult(
            ResetPasswordModel model,
            AspNetUser user,
            PasswordResetToken token,
            Uri uri,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IPasswordService> mockPasswordService,
            [Frozen] Mock<IPasswordResetCallback> mockPasswordResetCallback,
            UsersController controller)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            mockPasswordService
                .Setup(x => x.GeneratePasswordResetTokenAsync(user.Email))
                .ReturnsAsync(token);

            mockPasswordResetCallback
                .Setup(x => x.GetPasswordResetCallback(token))
                .Returns(uri);

            mockPasswordService
                .Setup(x => x.SendResetEmailAsync(user, uri))
                .Returns(Task.CompletedTask);

            var result = await controller.ResetPassword(UserId, model);

            mockUsersService.VerifyAll();
            mockPasswordService.VerifyAll();
            mockPasswordResetCallback.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Details));
            actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "userId", UserId },
            });
        }
    }
}
