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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_GetsAllOrganisations(
            IList<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetAllOrganisations().Returns(organisations);

            await controller.Index();

            await mockOrganisationService.Received().GetAllOrganisations();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel(
            IList<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetAllOrganisations().Returns(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ValidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetOrganisationsBySearchTerm(searchTerm).Returns(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task Get_Index_InvalidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Organisation> organisations,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetAllOrganisations().Returns(organisations);

            var model = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<IndexModel>().Organisations.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            List<Organisation> organisations,
            string searchTerm,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetOrganisationsBySearchTerm(searchTerm).Returns(organisations);

            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            foreach (var org in organisations)
            {
                actualResult.Should().Contain(x => x.Title == org.Name && x.Category == org.ExternalIdentifier);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] IOrganisationsService mockOrganisationService,
            OrganisationsController controller)
        {
            mockOrganisationService.GetOrganisationsBySearchTerm(searchTerm).Returns(new List<Organisation>());

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
            OrganisationsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_Find_NullOrganisation_ReturnsExpectedResult(
            FindOrganisationModel model,
            [Frozen] IOdsService mockOdsService,
            string error,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(model.OdsCode).Returns((null, error));
            var result = (await controller.Find(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Find_ExistingOrganisation_ReturnsExpectedResult(
            FindOrganisationModel model,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(model.OdsCode).Returns((organisation, null));

            mockOrganisationsService.OrganisationExists(organisation).Returns(true);

            var result = (await controller.Find(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var actualModel = result.Model.Should().BeAssignableTo<FindOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Find_ReturnsRedirectToActionResult(
            FindOrganisationModel model,
            [Frozen] IOdsService mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(model.OdsCode).Returns((organisation, null));
            var result = (await controller.Find(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Select));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "ods", model.OdsCode },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Select_ReturnsExpectedResult(
            string ods,
            [Frozen] IOdsService mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(ods).Returns((organisation, null));

            var result = (await controller.Select(ods)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<SelectOrganisationModel>().Subject;

            model.Should().BeEquivalentTo(new SelectOrganisationModel(organisation), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_Create_ReturnsExpectedResult(
            string ods,
            [Frozen] IOdsService mockOdsService,
            OdsOrganisation organisation,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(ods).Returns((organisation, null));

            var result = (await controller.Create(ods)).As<ViewResult>();
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<CreateOrganisationModel>().Subject;

            model.Should().BeEquivalentTo(new CreateOrganisationModel(organisation), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_Create_Failed_ReturnsRedirectToActionResult(
            CreateOrganisationModel model,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService mockOrgService,
            OdsOrganisation organisation,
            string error,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode).Returns((organisation, null));

            mockOrgService.AddOrganisation(organisation).Returns((0, error));

            var result = (await controller.Create(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Error));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "odsCode", model.OdsOrganisation.OdsCode },
                { "error", error },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Create_ReturnsRedirectToActionResult(
            CreateOrganisationModel model,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService mockOrgService,
            OdsOrganisation organisation,
            int orgId,
            OrganisationsController controller)
        {
            mockOdsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode).Returns((organisation, null));

            mockOrgService.AddOrganisation(organisation).Returns((orgId, null));

            var result = (await controller.Create(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Confirmation));

            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", orgId },
            });
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_Confirmation_ReturnsExpectedResult(
            [Frozen] IOrganisationsService mockOrgService,
            Organisation organisation,
            OrganisationsController controller)
        {
            mockOrgService.GetOrganisation(organisation.Id).Returns(organisation);

            var result = (await controller.Confirmation(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model.Should().BeAssignableTo<ConfirmationModel>().Subject;

            model.Should().BeEquivalentTo(new ConfirmationModel(organisation.Name), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Users_ReturnsExpectedResult(
            Organisation organisation,
            List<AspNetUser> users,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IUsersService mockUsersService,
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetAllUsersForOrganisation(organisation.Id).Returns(users);

            var result = (await controller.Users(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.Users));

            var model = result.Model.Should().BeAssignableTo<UsersModel>().Subject;

            model.OrganisationId.Should().Be(organisation.Id);
            model.OrganisationName.Should().Be(organisation.Name);
            model.Users.Should().BeEquivalentTo(users);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddUser_ReturnsExpectedResult(
            Organisation organisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            var result = (await controller.AddUser(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
        }

        [Theory]
        [MockAutoData]
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
        [MockInlineAutoData(true, "Buyer")]
        [MockInlineAutoData(false, "AccountManager")]
        public static async Task Post_AddUser_ValidModel_ReturnsExpectedResult(
            bool isDefaultAccountType,
            string accountType,
            int organisationId,
            UserDetailsModel model,
            [Frozen] ICreateUserService mockCreateBuyerService,
            OrganisationsController controller)
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
                    model.IsActive!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Users));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditUser_ReturnsExpectedResult(
           Organisation organisation,
           AspNetUser user,
           [Frozen] IUsersService mockUsersService,
           [Frozen] IOrganisationsService mockOrganisationsService,
           OrganisationsController controller)
        {
            user.PrimaryOrganisationId = organisation.Id;

            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockUsersService.GetUser(user.Id).Returns(user);

            var result = (await controller.EditUser(organisation.Id, user.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationBase/UserDetails");

            var model = result.Model.Should().BeAssignableTo<UserDetailsModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.UserId.Should().Be(user.Id);
            model.Title.Should().Be("Edit user");
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_EditUser_ValidModel_ReturnsExpectedResult(
            int organisationId,
            AspNetUser user,
            UserDetailsModel model,
            [Frozen] IUsersService mockUsersService,
            OrganisationsController controller)
        {
            user.PrimaryOrganisationId = organisationId;
            model.EmailAddress = "a@b.com";

            mockUsersService.GetUser(user.Id).Returns(user);

            mockUsersService.UpdateUser(user.Id, model.FirstName, model.LastName, model.EmailAddress, !model.IsActive!.Value, model.SelectedAccountType, organisationId).Returns(Task.CompletedTask);

            var result = (await controller.EditUser(organisationId, user.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.Users));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RelatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation relatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetRelatedOrganisations(organisation.Id).Returns(new List<Organisation> { relatedOrganisation });

            var result = (await controller.RelatedOrganisations(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RelatedOrganisations));

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
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetOrganisation(relatedOrganisation.Id).Returns(relatedOrganisation);

            var result = (await controller.RemoveRelatedOrganisation(organisation.Id, relatedOrganisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RemoveRelatedOrganisation));

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
            OrganisationsController controller)
        {
            mockOrganisationsService.RemoveRelatedOrganisations(model.OrganisationId, model.RelatedOrganisationId).Returns(Task.CompletedTask);

            var result = await controller.RemoveRelatedOrganisation(
                model.OrganisationId,
                model.RelatedOrganisationId,
                model);

            result.Should().NotBeNull();

            var redirectResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be(nameof(OrganisationsController.RelatedOrganisations));
            redirectResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int>
            {
                { "organisationId", model.OrganisationId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NominatedOrganisations_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetNominatedOrganisations(organisation.Id).Returns(new List<Organisation> { nominatedOrganisation });

            var result = (await controller.NominatedOrganisations(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.NominatedOrganisations));

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
            OrganisationsController controller)
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

            var result = (await controller.AddNominatedOrganisation(organisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.AddNominatedOrganisation));

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
            OrganisationsController controller)
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
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.AddNominatedOrganisation));

            var actualModel = result.Model.Should().BeAssignableTo<AddNominatedOrganisationModel>().Subject;

            actualModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddNominatedOrganisation_ValidModel_ReturnsExpectedResult(
            int nominatedOrganisationId,
            AddNominatedOrganisationModel model,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationsController controller)
        {
            model.SelectedOrganisationId = $"{nominatedOrganisationId}";

            mockOrganisationsService.AddNominatedOrganisation(model.OrganisationId, nominatedOrganisationId).Returns(Task.CompletedTask);

            var result = (await controller.AddNominatedOrganisation(model.OrganisationId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrganisationsController.NominatedOrganisations));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RemoveNominatedOrganisation_ReturnsExpectedResult(
            Organisation organisation,
            Organisation nominatedOrganisation,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrganisationsController controller)
        {
            mockOrganisationsService.GetOrganisation(organisation.Id).Returns(organisation);

            mockOrganisationsService.GetOrganisation(nominatedOrganisation.Id).Returns(nominatedOrganisation);

            var result = (await controller.RemoveNominatedOrganisation(organisation.Id, nominatedOrganisation.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be(typeof(OrganisationBaseController).ControllerName() + "/" + nameof(OrganisationBaseController.RemoveNominatedOrganisation));

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
            OrganisationsController controller)
        {
            mockOrganisationsService.RemoveNominatedOrganisation(model.OrganisationId, model.NominatedOrganisationId).Returns(Task.CompletedTask);

            var result = await controller.RemoveNominatedOrganisation(
                model.OrganisationId,
                model.NominatedOrganisationId,
                model);

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
