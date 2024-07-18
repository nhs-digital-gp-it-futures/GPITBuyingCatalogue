using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UsersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_GetsAllUsers(
            List<AspNetUser> users,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetAllUsers().Returns(users);

            var result = await controller.Index();

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
        [MockAutoData]
        public static async Task Get_Index_WithSearchTerm_GetsAllMatchingUsers(
            string searchTerm,
            List<AspNetUser> users,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetAllUsersBySearchTerm(searchTerm).Returns(users);

            var result = await controller.Index(search: searchTerm);

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
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            List<AspNetUser> users,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetAllUsersBySearchTerm(searchTerm).Returns(users);

            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            foreach (var user in users)
            {
                actualResult.Should().Contain(x => x.Title == user.FullName && x.Category == user.Email);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetAllUsersBySearchTerm(searchTerm).Returns(new List<AspNetUser>());

            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task Get_SearchResults_InvalidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] IUsersService usersService,
            UsersController controller)
        {
            usersService.GetAllUsers().Returns(Enumerable.Empty<AspNetUser>().ToList());

            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Add_ReturnsExpectedResult(
            List<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationsService,
            UsersController controller)
        {
            mockOrganisationsService.GetAllOrganisations().Returns(organisations);

            var result = await controller.Add();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            actual.ViewName.Should().Be("UserDetails");

            var model = actual.Model.Should().BeOfType<UserDetailsModel>().Subject;

            foreach (var organisation in organisations)
            {
                model.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }

            model.Title.Should().Contain("Add user");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Add_WithModelErrors_ReturnsExpectedResult(
            List<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationsService,
            UserDetailsModel model,
            UsersController controller)
        {
            mockOrganisationsService.GetAllOrganisations().Returns(organisations);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.Add(model);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            actual.ViewName.Should().Be("UserDetails");
            var actualModel = actual.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Add_ReturnsExpectedResult(
            [Frozen] ICreateUserService mockCreateUserService,
            UserDetailsModel model,
            UsersController controller)
        {
            model.SelectedOrganisationId = OrganisationConstants.NhsDigitalOrganisationId;

            mockCreateUserService.Create(
                    OrganisationConstants.NhsDigitalOrganisationId,
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.SelectedAccountType,
                    !model.IsActive!.Value).Returns(new AspNetUser());

            var result = await controller.Add(model);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_ReturnsExpectedResult(
            List<Organisation> organisations,
            AspNetUser user,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            UsersController controller)
        {
            mockOrganisationsService.GetAllOrganisations().Returns(organisations);

            mockUsersService.GetUser(user.Id).Returns(user);

            var result = (await controller.Edit(user.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            foreach (var organisation in organisations)
            {
                model.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }

            model.UserId.Should().Be(user.Id);
            model.Title.Should().Be("Edit user");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_NullUser_ReturnsExpectedResult(
            List<Organisation> organisations,
            AspNetUser user,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            UsersController controller)
        {
            mockOrganisationsService.GetAllOrganisations().Returns(organisations);

            mockUsersService.GetUser(user.Id).Returns((AspNetUser)null);

            var result = (await controller.Edit(user.Id)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_WithModelErrors_ReturnsExpectedResult(
            int userId,
            UserDetailsModel model,
            UsersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.Edit(userId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("UserDetails");

            var actualModel = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_NullUser_ReturnsExpectedResult(
            int userId,
            UserDetailsModel model,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetUser(userId).Returns((AspNetUser)null);

            var result = (await controller.Edit(userId, model)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_ValidModel_ReturnsExpectedResult(
            int userId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] IUsersService mockUsersService,
            UsersController controller)
        {
            mockUsersService.GetUser(userId).Returns(user);

            model.SelectedOrganisationId = 1;

            mockUsersService.UpdateUser(userId, model.FirstName, model.LastName, model.Email, !model.IsActive!.Value, model.SelectedAccountType, model.SelectedOrganisationId!.Value).Returns(Task.CompletedTask);

            var result = (await controller.Edit(userId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(UsersController.Index));
        }
    }
}
