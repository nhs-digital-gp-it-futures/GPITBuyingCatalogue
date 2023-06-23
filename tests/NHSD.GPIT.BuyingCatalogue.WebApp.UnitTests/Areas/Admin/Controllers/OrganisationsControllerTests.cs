using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using NuGet.Packaging.Signing;
using Xunit;
using OrganisationModel = NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels.OrganisationModel;

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
                .Value.As<IEnumerable<SuggestionSearchResult>>()
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
            OrganisationsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Find_ReturnsExpectedResult(
            string ods,
            OrganisationsController controller)
        {
            var result = controller.Find(ods).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            model.Should().BeEquivalentTo(new FindOrganisationModel(ods), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Find_WithModelErrors_ReturnsExpectedResult(
            FindOrganisationModel model,
            OrganisationsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.Find(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Find_NullOrganisation_ReturnsExpectedResult(
            FindOrganisationModel model,
            [Frozen] Mock<IOdsService> mockOdsService,
            string error,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(model.OdsCode))
                .ReturnsAsync((null, error));
            var result = (await controller.Find(model)).As<ViewResult>();

            mockOdsService.VerifyAll();
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Find_ExistingOrganisation_ReturnsExpectedResult(
            FindOrganisationModel model,
            [Frozen] Mock<IOdsService> mockOdsService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(model.OdsCode))
                .ReturnsAsync((organisation, null));

            mockOrganisationsService
                .Setup(x => x.OrganisationExists(organisation))
                .ReturnsAsync(true);

            var result = (await controller.Find(model)).As<ViewResult>();

            mockOdsService.VerifyAll();
            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Find_ReturnsRedirectToActionResult(
            FindOrganisationModel model,
            [Frozen] Mock<IOdsService> mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(model.OdsCode))
                .ReturnsAsync((organisation, null));
            var result = (await controller.Find(model)).As<RedirectToActionResult>();

            mockOdsService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Select));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "ods", model.OdsCode },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Select_ReturnsExpectedResult(
            string ods,
            [Frozen] Mock<IOdsService> mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(ods))
                .ReturnsAsync((organisation, null));

            var result = (await controller.Select(ods)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<SelectOrganisationModel>().Subject;

            model.Should().BeEquivalentTo(new SelectOrganisationModel(organisation), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Select_WithModelErrors_ReturnsExpectedResult(
            SelectOrganisationModel model,
            OrganisationsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.Select(model).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<SelectOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Select_ReturnsRedirectToActionResult(
            SelectOrganisationModel model,
            OrganisationsController controller)
        {
            var result = controller.Select(model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Create));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "ods", model.OdsOrganisation.OdsCode },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Create_ReturnsExpectedResult(
            string ods,
            [Frozen] Mock<IOdsService> mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(ods))
                .ReturnsAsync((organisation, null));

            var result = (await controller.Create(ods)).As<ViewResult>();

            mockOdsService.VerifyAll();
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<CreateOrganisationModel>().Subject;

            model.Should().BeEquivalentTo(new CreateOrganisationModel(organisation), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Create_WithModelErrors_ReturnsExpectedResult(
            CreateOrganisationModel model,
            OrganisationsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = (await controller.Create(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<CreateOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Create_Failed_ReturnsRedirectToActionResult(
            CreateOrganisationModel model,
            [Frozen] Mock<IOdsService> mockOdsService,
            [Frozen] Mock<IOrganisationsService> mockOrgService,
            OdsOrganisation organisation,
            string error,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode))
                .ReturnsAsync((organisation, null));

            mockOrgService
                .Setup(x => x.AddOrganisation(organisation))
                .ReturnsAsync((0, error));

            var result = (await controller.Create(model)).As<RedirectToActionResult>();

            mockOdsService.VerifyAll();
            mockOrgService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Error));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "odsCode", model.OdsOrganisation.OdsCode },
                { "error", error },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Create_ReturnsRedirectToActionResult(
            CreateOrganisationModel model,
            [Frozen] Mock<IOdsService> mockOdsService,
            [Frozen] Mock<IOrganisationsService> mockOrgService,
            OdsOrganisation organisation,
            int orgId,
            OrganisationsController controller)
        {
            mockOdsService
                .Setup(x => x.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode))
                .ReturnsAsync((organisation, null));

            mockOrgService
                .Setup(x => x.AddOrganisation(organisation))
                .ReturnsAsync((orgId, null));

            var result = (await controller.Create(model)).As<RedirectToActionResult>();

            mockOdsService.VerifyAll();
            mockOrgService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Confirmation));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", orgId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Error_ReturnsExpectedResult(
            string ods,
            string error,
            OrganisationsController controller)
        {
            var result = controller.Error(ods, error).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<AddAnOrganisationErrorModel>().Subject;

            model.Should().BeEquivalentTo(new AddAnOrganisationErrorModel(ods, error), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Confirmation_ReturnsExpectedResult(
            [Frozen] Mock<IOrganisationsService> mockOrgService,
            Organisation organisation,
            OrganisationsController controller)
        {
            mockOrgService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            var result = (await controller.Confirmation(organisation.Id)).As<ViewResult>();

            mockOrgService.VerifyAll();
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<ConfirmationModel>().Subject;

            model.Should().BeEquivalentTo(new ConfirmationModel(organisation.Name), opt => opt.Excluding(m => m.BackLink));
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
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            UserDetailsModel model,
            OrganisationsController controller)
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
            OrganisationsController controller)
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
                    model.IsActive!.Value));

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Users));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditUser_ReturnsExpectedResult(
           Organisation organisation,
           AspNetUser user,
           [Frozen] Mock<IUsersService> mockUsersService,
           [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
           OrganisationsController controller)
        {
            user.PrimaryOrganisationId = organisation.Id;

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockUsersService
                .Setup(x => x.GetUser(user.Id))
                .ReturnsAsync(user);

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
        public static async Task Post_EditUser_WithModelErrors_ReturnsExpectedResult(
            int organisationId,
            int userId,
            UserDetailsModel model,
            OrganisationsController controller)
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
            OrganisationsController controller)
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_NominatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
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
            OrganisationsController controller)
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
            OrganisationsController controller)
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
            OrganisationsController controller)
        {
            model.SelectedOrganisationId = $"{nominatedOrganisationId}";

            mockOrganisationsService
                .Setup(x => x.AddNominatedOrganisation(model.OrganisationId, nominatedOrganisationId))
                .Returns(Task.CompletedTask);

            var result = (await controller.AddNominatedOrganisation(model.OrganisationId, model)).As<RedirectToActionResult>();

            mockOrganisationsService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.NominatedOrganisations));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_RemoveNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisation.Id))
                .ReturnsAsync(organisation);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(nominatedOrganisation.Id))
                .ReturnsAsync(nominatedOrganisation);

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
            OrganisationsController controller)
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

            redirectResult.ActionName.Should().Be(nameof(OrganisationsController.NominatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }
    }
}
