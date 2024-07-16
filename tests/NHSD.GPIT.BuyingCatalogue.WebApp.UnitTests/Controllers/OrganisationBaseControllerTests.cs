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
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationControllerStub).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Details_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetRelatedOrganisations(organisation.Id).Returns(relatedOrganisations);

            mockUsersService.GetAllUsersForOrganisation(organisation.Id).Returns(users);

            controller.Url = mockUrlHelper;

            var result = (await controller.Details(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.Details)}");

            var model = result.Model.Should().BeAssignableTo<DetailsModel>().Subject;

            model.Organisation.Should().NotBeNull();
            model.Organisation.Id.Should().Be(organisation.Id);
            model.Organisation.Name.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
            model.RelatedOrganisations.Should().BeEquivalentTo(relatedOrganisations);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Users_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetAllUsersForOrganisation(organisation.Id).Returns(users);

            controller.Url = mockUrlHelper;

            var result = (await controller.Users(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.Users)}");

            var model = result.Model.Should().BeAssignableTo<UsersModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddUser_ReturnsExpectedResult(
            Organisation organisation,
            bool isAccountManagerLimit,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.IsAccountManagerLimit(organisation.Id, 0).Returns(isAccountManagerLimit);

            controller.Url = mockUrlHelper;

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.IsDefaultAccountType.Should().Be(isAccountManagerLimit);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            UserDetailsModel model,
            OrganisationControllerStub controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.AddUser(organisationId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/UserDetails");

            var actualModel = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(true, "Buyer")]
        [MockInlineAutoData(false, "AccountManager")]
        public static async Task Post_AddUser_ValidModel_ReturnsExpectedResult(
            bool isDefaultAccountType,
            string accountType,
            int organisationId,
            UserDetailsModel model,
            [Frozen] ICreateUserService mockCreateBuyerService,
            OrganisationControllerStub controller)
        {
            model.EmailAddress = "a@b.com";
            model.SelectedAccountType = "AccountManager";
            model.IsDefaultAccountType = isDefaultAccountType;
            model.IsActive = true;

            var result = (await controller.AddUser(organisationId, model)).As<RedirectToActionResult>();

            await mockCreateBuyerService.Received().Create(
                    organisationId,
                    model.FirstName,
                    model.LastName,
                    model.EmailAddress,
                    accountType,
                    model.IsActive.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationControllerStub.Users));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_ReturnsExpectedResult(
             Organisation organisation,
             AspNetUser user,
             [Frozen] IUsersService mockUsersService,
             [Frozen] IOrganisationsService mockOrganisationsService,
             [Frozen] IUrlHelper mockUrlHelper,
             OrganisationControllerStub controller)
        {
            user.PrimaryOrganisationId = organisation.Id;

            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetUser(user.Id).Returns(user);

            controller.Url = mockUrlHelper;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.UserId.Should().Be(user.Id);
            model.Title.Should().Be("Edit user");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_InvalidOrganisationId_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetUser(user.Id).Returns(user);

            controller.Url = mockUrlHelper;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_NullOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns((Organisation)null);

            mockUsersService.GetUser(user.Id).Returns(user);

            controller.Url = mockUrlHelper;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_NullOUser_ReturnsExpectedResult(
            Organisation organisation,
            AspNetUser user,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetUser(user.Id).Returns((AspNetUser)null);

            controller.Url = mockUrlHelper;

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            int userId,
            UserDetailsModel model,
            OrganisationControllerStub controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.EditUser(organisationId, userId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/UserDetails");

            var actualModel = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_ValidModel_ReturnsExpectedResult(
            int organisationId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] IUsersService mockUsersService,
            OrganisationControllerStub controller)
        {
            user.PrimaryOrganisationId = organisationId;
            model.EmailAddress = "a@b.com";

            mockUsersService.GetUser(user.Id).Returns(user);

            mockUsersService.UpdateUser(user.Id, model.FirstName, model.LastName, model.EmailAddress, !model.IsActive!.Value, model.SelectedAccountType, organisationId).Returns(Task.CompletedTask);

            var result = (await controller.EditUser(organisationId, user.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationControllerStub.Users));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditUser_InvalidOrganisationId_ReturnsExpectedResult(
            int organisationId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] IUsersService mockUsersService,
            OrganisationControllerStub controller)
        {
            model.EmailAddress = "a@b.com";

            mockUsersService.GetUser(user.Id).Returns(user);

            var result = (await controller.EditUser(organisationId, user.Id, model)).As<BadRequestResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RelatedOrganisations_GPOrganisation_ReturnsBadRequest(
            int organisationId,
            Organisation organisation,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationControllerStub controller)
        {
            organisation.OrganisationType = OrganisationType.GP;

            mockOrganisationService.GetOrganisation(organisationId).Returns(organisation);

            var result = (await controller.RelatedOrganisations(organisationId)).As<BadRequestResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RelatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetRelatedOrganisations(organisation.Id).Returns(new List<Organisation> { relatedOrganisation });

            var result = (await controller.RelatedOrganisations(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.RelatedOrganisations)}");

            var model = result.Model.Should().BeAssignableTo<RelatedOrganisationsModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.RelatedOrganisations.Should().BeEquivalentTo(new[] { relatedOrganisation });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RemoveRelatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetOrganisation(relatedOrganisation.Id).Returns(relatedOrganisation);

            controller.Url = mockUrlHelper;

            var result = (await controller.RemoveRelatedOrganisation(organisation.Id, relatedOrganisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.RemoveRelatedOrganisation)}");

            var model = result.Model.Should().BeAssignableTo<RemoveRelatedOrganisationModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.RelatedOrganisationId.Should().Be(relatedOrganisation.Id);
            model.RelatedOrganisationName.Should().Be(relatedOrganisation.Name);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_RemoveRelatedOrganisation_ReturnsExpectedResult(
            RemoveRelatedOrganisationModel model,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.RemoveRelatedOrganisations(model.OrganisationId, model.RelatedOrganisationId).Returns(Task.CompletedTask);

            var result = await controller.RemoveRelatedOrganisation(
                model.OrganisationId,
                model.RelatedOrganisationId,
                model);

            result.Should().NotBeNull();

            var redirectResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be(nameof(OrganisationControllerStub.RelatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NominatedOrganisations_GPOrganisation_ReturnsBadRequest(
            int organisationId,
            Organisation organisation,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationControllerStub controller)
        {
            organisation.OrganisationType = OrganisationType.GP;

            mockOrganisationService.GetOrganisation(organisationId).Returns(organisation);

            var result = (await controller.NominatedOrganisations(organisationId)).As<BadRequestResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NominatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetNominatedOrganisations(organisation.Id).Returns(new List<Organisation> { nominatedOrganisation });

            var result = (await controller.NominatedOrganisations(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.NominatedOrganisations)}");

            var model = result.Model.Should().BeAssignableTo<NominatedOrganisationsModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.NominatedOrganisations.Should().BeEquivalentTo(new[] { nominatedOrganisation });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            Organisation potentialOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService
                .GetAllOrganisations()
                .Returns(new List<Organisation>
                {
                    organisation,
                    nominatedOrganisation,
                    potentialOrganisation,
                });

            mockOrganisationsService.GetNominatedOrganisations(organisation.Id).Returns(new List<Organisation> { nominatedOrganisation });

            controller.Url = mockUrlHelper;

            var result = (await controller.AddNominatedOrganisation(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.AddNominatedOrganisation)}");

            var model = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;
            var expected = new SelectOption<string>(potentialOrganisation.Name, $"{potentialOrganisation.Id}");

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.PotentialOrganisations.Should().BeEquivalentTo(new[] { expected });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddNominatedOrganisation_WithModelErrors_ReturnsExpectedResult(
            Organisation organisation,
            Organisation potentialOrganisation,
            AddNominatedOrganisationModel model,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetAllOrganisations().Returns(new List<Organisation> { potentialOrganisation });

            mockOrganisationsService.GetNominatedOrganisations(organisation.Id).Returns(new List<Organisation>());

            model.SelectedOrganisationId = string.Empty;
            model.PotentialOrganisations = new[]
            {
                new SelectOption<string>(potentialOrganisation.Name, $"{potentialOrganisation.Id}"),
            };

            var result = (await controller.AddNominatedOrganisation(organisation.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.AddNominatedOrganisation)}");

            var actualModel = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddNominatedOrganisation_ValidModel_ReturnsExpectedResult(
            int nominatedOrganisationId,
            AddNominatedOrganisationModel model,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            model.SelectedOrganisationId = $"{nominatedOrganisationId}";

            mockOrganisationsService.AddNominatedOrganisation(model.OrganisationId, nominatedOrganisationId).Returns(Task.CompletedTask);

            var result = (await controller.AddNominatedOrganisation(model.OrganisationId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationControllerStub.NominatedOrganisations));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RemoveNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUrlHelper mockUrlHelper,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetOrganisation(nominatedOrganisation.Id).Returns(nominatedOrganisation);

            controller.Url = mockUrlHelper;

            var result = (await controller.RemoveNominatedOrganisation(organisation.Id, nominatedOrganisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be($"{OrganisationBaseController.ViewBaseName}/{nameof(OrganisationControllerStub.RemoveNominatedOrganisation)}");

            var model = result.Model.Should().BeAssignableTo<RemoveNominatedOrganisationModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.NominatedOrganisationId.Should().Be(nominatedOrganisation.Id);
            model.NominatedOrganisationName.Should().Be(nominatedOrganisation.Name);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_RemoveNominatedOrganisation_ReturnsExpectedResult(
            RemoveNominatedOrganisationModel model,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationControllerStub controller)
        {
            mockOrganisationsService.RemoveNominatedOrganisation(model.OrganisationId, model.NominatedOrganisationId).Returns(Task.CompletedTask);

            var result = await controller.RemoveNominatedOrganisation(
                model.OrganisationId,
                model.NominatedOrganisationId,
                model);

            result.Should().NotBeNull();

            var redirectResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be(nameof(OrganisationControllerStub.NominatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }

        public class OrganisationControllerStub(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService,
            AccountManagementSettings accountManagementSettings)
            : OrganisationBaseController(
                organisationsService,
                odsService,
                createBuyerService,
                userService,
                accountManagementSettings)
        {
            protected override string ControllerName => nameof(OrganisationControllerStub);

            protected override string HomeLink => string.Empty;
        }
    }
}
