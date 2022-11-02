using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class OrganisationControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Details_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetRelatedOrganisations(organisation.Id))
                .ReturnsAsync(relatedOrganisations);

            mockUsersService
                .Setup(x => x.GetAllUsersForOrganisation(organisation.Id))
                .ReturnsAsync(users);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.Details(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.Details));

            var model = result.Model.Should().BeAssignableTo<DetailsModel>().Subject;

            model.Organisation.Should().NotBeNull();
            model.Organisation.Id.Should().Be(organisation.Id);
            model.Organisation.Name.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
            model.RelatedOrganisations.Should().BeEquivalentTo(relatedOrganisations);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Users_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetAllUsersForOrganisation(organisation.Id))
                .ReturnsAsync(users);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.Users(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.Users));

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
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.AddUser));

            var model = result.Model.Should().BeAssignableTo<AddUserModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            AddUserModel model,
            OrganisationController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.AddUser(organisationId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.AddUser));

            var actualModel = result.Model.Should().BeAssignableTo<AddUserModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_ValidModel_ReturnsExpectedResult(
            int organisationId,
            AddUserModel model,
            [Frozen] Mock<ICreateUserService> mockCreateBuyerService,
            OrganisationController controller)
        {
            model.EmailAddress = "a@b.com";

            mockCreateBuyerService
                .Setup(x => x.Create(organisationId, model.FirstName, model.LastName, model.EmailAddress, OrganisationFunction.Buyer.Name))
                .ReturnsAsync((AspNetUser)null);

            var result = (await controller.AddUser(organisationId, model)).As<RedirectToActionResult>();

            mockCreateBuyerService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_UserStatus_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.UserStatus(organisation.Id, user.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.UserStatus));

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
            OrganisationController controller)
        {
            mockUsersService
                .Setup(x => x.EnableOrDisableUser(userId, viewModel.IsActive))
                .Returns(Task.CompletedTask);

            var result = (await controller.UserStatus(organisationId, userId, viewModel)).As<RedirectToActionResult>();

            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RelatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.RelatedOrganisations));

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
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(relatedOrganisation.Id))
                .ReturnsAsync(relatedOrganisation);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.RemoveRelatedOrganisation(organisation.Id, relatedOrganisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.RemoveRelatedOrganisation));

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
            OrganisationController controller)
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

            redirectResult.ActionName.Should().Be(nameof(OrganisationController.RelatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NominatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetNominatedOrganisations(organisation.Id))
                .ReturnsAsync(new List<Organisation> { nominatedOrganisation });

            var result = (await controller.NominatedOrganisations(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.NominatedOrganisations));

            var model = result.Model.Should().BeAssignableTo<NominatedOrganisationsModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.NominatedOrganisations.Should().BeEquivalentTo(new[] { nominatedOrganisation });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            Organisation potentialOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(new List<Organisation>
                {
                    organisation,
                    nominatedOrganisation,
                    potentialOrganisation,
                });

            mockOrganisationsService
                .Setup(x => x.GetNominatedOrganisations(organisation.Id))
                .ReturnsAsync(new List<Organisation> { nominatedOrganisation });

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.AddNominatedOrganisation(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.AddNominatedOrganisation));

            var model = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;
            var expected = new SelectListItem(potentialOrganisation.Name, $"{potentialOrganisation.Id}");

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.PotentialOrganisations.Should().BeEquivalentTo(new[] { expected });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddNominatedOrganisation_WithModelErrors_ReturnsExpectedResult(
            Organisation organisation,
            Organisation potentialOrganisation,
            AddNominatedOrganisationModel model,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(new List<Organisation> { potentialOrganisation });

            mockOrganisationsService
                .Setup(x => x.GetNominatedOrganisations(organisation.Id))
                .ReturnsAsync(new List<Organisation>());

            model.SelectedOrganisationId = string.Empty;
            model.PotentialOrganisations = new[]
            {
                new SelectListItem(potentialOrganisation.Name, $"{potentialOrganisation.Id}"),
            };

            var result = (await controller.AddNominatedOrganisation(organisation.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.AddNominatedOrganisation));

            var actualModel = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddNominatedOrganisation_ValidModel_ReturnsExpectedResult(
            int nominatedOrganisationId,
            AddNominatedOrganisationModel model,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationController controller)
        {
            model.SelectedOrganisationId = $"{nominatedOrganisationId}";

            mockOrganisationsService
                .Setup(x => x.AddNominatedOrganisation(model.OrganisationId, nominatedOrganisationId))
                .Returns(Task.CompletedTask);

            var result = (await controller.AddNominatedOrganisation(model.OrganisationId, model)).As<RedirectToActionResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationController.NominatedOrganisations));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RemoveNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(nominatedOrganisation.Id))
                .ReturnsAsync(nominatedOrganisation);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.RemoveNominatedOrganisation(organisation.Id, nominatedOrganisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationController).ControllerName() + "/" + nameof(OrganisationController.RemoveNominatedOrganisation));

            var model = result.Model.Should().BeAssignableTo<RemoveNominatedOrganisationModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.NominatedOrganisationId.Should().Be(nominatedOrganisation.Id);
            model.NominatedOrganisationName.Should().Be(nominatedOrganisation.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_RemoveNominatedOrganisation_ReturnsExpectedResult(
            RemoveNominatedOrganisationModel model,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationController controller)
        {
            mockOrganisationsService
                .Setup(x => x.RemoveNominatedOrganisation(model.OrganisationId, model.NominatedOrganisationId))
                .Returns(Task.CompletedTask);

            var result = await controller.RemoveNominatedOrganisation(
                model.OrganisationId,
                model.NominatedOrganisationId,
                model);

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();

            var redirectResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be(nameof(OrganisationController.NominatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }
    }
}
