﻿using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Autocomplete;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class OrganisationsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrganisationsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(OrganisationsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_GetsAllOrganisations(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            await controller.Index();

            mockOrganisationService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel(
            IList<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index()).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_Index_InvalidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            mockOrganisationService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            List<Organisation> organisations,
            string searchTerm,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(organisations);

            var result = await controller.SearchResults(searchTerm);

            mockOrganisationService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            foreach (var org in organisations)
            {
                actualResult.Should().Contain(x => x.Title == org.Name && x.Category == org.ExternalIdentifier);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationsBySearchTerm(searchTerm))
                .ReturnsAsync(new List<Organisation>());

            var result = await controller.SearchResults(searchTerm);

            mockOrganisationService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_SearchResults_InvalidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            OrganisationsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<AutocompleteResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Users_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetAllUsersForOrganisation(organisation.Id))
                .ReturnsAsync(users);

            var result = (await controller.Users(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();

            var model = result.Model.Should().BeAssignableTo<UsersModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddUser_ReturnsExpectedResult(
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();

            var model = result.Model.Should().BeAssignableTo<AddUserModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            AddUserModel model,
            OrganisationsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.AddUser(organisationId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<AddUserModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_ValidModel_ReturnsExpectedResult(
            int organisationId,
            AddUserModel model,
            [Frozen] Mock<ICreateBuyerService> mockCreateBuyerService,
            OrganisationsController controller)
        {
            model.EmailAddress = "a@b.com";

            mockCreateBuyerService
                .Setup(x => x.Create(organisationId, model.FirstName, model.LastName, model.TelephoneNumber, model.EmailAddress, false))
                .ReturnsAsync((AspNetUser)null);

            var result = (await controller.AddUser(organisationId, model)).As<RedirectToActionResult>();

            mockCreateBuyerService.VerifyAll();

            result.Should().NotBeNull();
            result.ControllerName.Should().Be(typeof(OrganisationsController).ControllerName());
            result.ActionName.Should().Be(nameof(OrganisationsController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_UserStatus_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            var result = (await controller.UserStatus(organisation.Id, user.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<UserStatusModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.UserId.Should().Be(user.Id);
            model.UserEmail.Should().Be(user.Email);
            model.IsActive.Should().Be(!user.Disabled);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_UserStatus_ReturnsExpectedResult(
            int organisationId,
            int userId,
            UserStatusModel viewModel,
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationsController controller)
        {
            mockUsersService
                .Setup(x => x.EnableOrDisableUser(userId, viewModel.IsActive))
                .Returns(Task.CompletedTask);

            var result = (await controller.UserStatus(organisationId, userId, viewModel)).As<RedirectToActionResult>();

            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RelatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetRelatedOrganisations(organisation.Id))
                .ReturnsAsync(new List<Organisation> { relatedOrganisation });

            var result = (await controller.RelatedOrganisations(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();

            var model = result.Model.Should().BeAssignableTo<RelatedOrganisationsModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.RelatedOrganisations.Should().BeEquivalentTo(new[] { relatedOrganisation });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RemoveRelatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(relatedOrganisation.Id))
                .ReturnsAsync(relatedOrganisation);

            var result = (await controller.RemoveRelatedOrganisation(organisation.Id, relatedOrganisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();

            var model = result.Model.Should().BeAssignableTo<RemoveRelatedOrganisationModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.RelatedOrganisationId.Should().Be(relatedOrganisation.Id);
            model.RelatedOrganisationName.Should().Be(relatedOrganisation.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_RemoveRelatedOrganisation_ReturnsExpectedResult(
            RemoveRelatedOrganisationModel model,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.RemoveRelatedOrganisations(model.OrganisationId, model.RelatedOrganisationId))
                .Returns(Task.CompletedTask);

            var result = await controller.RemoveRelatedOrganisation(
                model.OrganisationId,
                model.RelatedOrganisationId,
                model);

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();

            var redirectResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be(nameof(OrganisationsController.RelatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }
    }
}
