﻿using System;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var model = actual.Model.Should().BeOfType<UserDetailsModel>().Subject;

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
            UserDetailsModel model,
            UsersController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.Add(model);

            mockOrganisationsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var actualModel = actual.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            foreach (var organisation in organisations)
            {
                actualModel.Organisations.Should().Contain(x => x.Value == $"{organisation.Id}" && x.Text == organisation.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Add_ReturnsExpectedResult(
            [Frozen] Mock<ICreateUserService> mockCreateUserService,
            UserDetailsModel model,
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
                    !model.IsActive!.Value))
                .ReturnsAsync(new AspNetUser());

            var result = await controller.Add(model);

            mockCreateUserService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(UsersController.Index));
        }
    }
}
