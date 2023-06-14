using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class OrganisationBaseControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationBaseController).GetConstructors();

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
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.Details));

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
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.Users));

            var model = result.Model.Should().BeAssignableTo<UsersModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddUser_ReturnsExpectedResult(
            Organisation organisation,
            bool isAccountManagerLimit,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationBaseController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.IsAccountManagerLimit(organisation.Id, 0))
                .ReturnsAsync(isAccountManagerLimit);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();
            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.IsDefaultAccountType.Should().Be(isAccountManagerLimit);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            UserDetailsModel model,
            OrganisationBaseController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.AddUser(organisationId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var actualModel = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonInlineAutoData(true, "Buyer")]
        [CommonInlineAutoData(false, "AccountManager")]
        public static async Task Post_AddUser_ValidModel_ReturnsExpectedResult(
            bool isDefaultAccountType,
            string accountType,
            int organisationId,
            UserDetailsModel model,
            [Frozen] Mock<ICreateUserService> mockCreateBuyerService,
            OrganisationBaseController controller)
        {
            model.EmailAddress = "a@b.com";
            model.SelectedAccountType = "AccountManager";
            model.IsDefaultAccountType = isDefaultAccountType;
            model.IsActive = true;

            var result = (await controller.AddUser(organisationId, model)).As<RedirectToActionResult>();

            mockCreateBuyerService.Verify(
                x => x.Create(
                    organisationId,
                    model.FirstName,
                    model.LastName,
                    model.EmailAddress,
                    accountType,
                    model.IsActive.Value),
                Times.Once());

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationBaseController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditUser_ReturnsExpectedResult(
             Organisation organisation,
             AspNetUser user,
             [Frozen] Mock<IUsersService> mockUsersService,
             [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
             [Frozen] Mock<IUrlHelper> mockUrlHelper,
             OrganisationBaseController controller)
        {
            user.PrimaryOrganisationId = organisation.Id;

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.UserId.Should().Be(user.Id);
            model.Title.Should().Be("Edit user");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditUser_InvalidOrganisationId_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationBaseController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditUser_NullOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationBaseController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync((Organisation)null);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditUser_NullOUser_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationBaseController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync((AspNetUser)null);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            int userId,
            UserDetailsModel model,
            OrganisationBaseController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.EditUser(organisationId, userId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var actualModel = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditUser_ValidModel_ReturnsExpectedResult(
            int organisationId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationBaseController controller)
        {
            user.PrimaryOrganisationId = organisationId;
            model.EmailAddress = "a@b.com";

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            mockUsersService
                .Setup(x => x.UpdateUser(user.Id, model.FirstName, model.LastName, model.EmailAddress, !model.IsActive!.Value, model.SelectedAccountType, organisationId))
                .Returns(Task.CompletedTask);

            var result = (await controller.EditUser(organisationId, user.Id, model)).As<RedirectToActionResult>();

            mockUsersService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationBaseController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditUser_InvalidOrganisationId_ReturnsExpectedResult(
            int organisationId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] Mock<IUsersService> mockUsersService,
            OrganisationBaseController controller)
        {
            model.EmailAddress = "a@b.com";

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

            var result = (await controller.EditUser(organisationId, user.Id, model)).As<BadRequestResult>();

            mockUsersService.VerifyAll();
            mockUsersService.VerifyNoOtherCalls();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RelatedOrganisations_GPOrganisation_ReturnsBadRequest(
            int organisationId,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationBaseController controller)
        {
            organisation.OrganisationType = OrganisationType.GP;

            mockOrganisationService
                .Setup(x => x.GetOrganisation(organisationId))
                .ReturnsAsync(organisation);

            var result = (await controller.RelatedOrganisations(organisationId)).As<BadRequestResult>();

            mockOrganisationService.VerifyAll();
            mockOrganisationService.VerifyNoOtherCalls();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RelatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RelatedOrganisations));

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
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RemoveRelatedOrganisation));

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
            OrganisationBaseController controller)
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

            redirectResult.ActionName.Should().Be(nameof(OrganisationBaseController.RelatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NominatedOrganisations_GPOrganisation_ReturnsBadRequest(
            int organisationId,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            OrganisationBaseController controller)
        {
            organisation.OrganisationType = OrganisationType.GP;

            mockOrganisationService
                .Setup(x => x.GetOrganisation(organisationId))
                .ReturnsAsync(organisation);

            var result = (await controller.NominatedOrganisations(organisationId)).As<BadRequestResult>();

            mockOrganisationService.VerifyAll();
            mockOrganisationService.VerifyNoOtherCalls();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NominatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.NominatedOrganisations));

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
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.AddNominatedOrganisation));

            var model = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;
            var expected = new SelectOption<string>(potentialOrganisation.Name, $"{potentialOrganisation.Id}");

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
            OrganisationBaseController controller)
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
                new SelectOption<string>(potentialOrganisation.Name, $"{potentialOrganisation.Id}"),
            };

            var result = (await controller.AddNominatedOrganisation(organisation.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.AddNominatedOrganisation));

            var actualModel = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddNominatedOrganisation_ValidModel_ReturnsExpectedResult(
            int nominatedOrganisationId,
            AddNominatedOrganisationModel model,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationBaseController controller)
        {
            model.SelectedOrganisationId = $"{nominatedOrganisationId}";

            mockOrganisationsService
                .Setup(x => x.AddNominatedOrganisation(model.OrganisationId, nominatedOrganisationId))
                .Returns(Task.CompletedTask);

            var result = (await controller.AddNominatedOrganisation(model.OrganisationId, model)).As<RedirectToActionResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationBaseController.NominatedOrganisations));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RemoveNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            OrganisationBaseController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RemoveNominatedOrganisation));

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
            OrganisationBaseController controller)
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

            redirectResult.ActionName.Should().Be(nameof(OrganisationBaseController.NominatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }
    }
}
