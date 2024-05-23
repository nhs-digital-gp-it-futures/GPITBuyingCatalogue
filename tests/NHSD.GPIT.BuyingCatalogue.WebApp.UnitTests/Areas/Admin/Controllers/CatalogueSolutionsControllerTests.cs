using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using NSubstitute;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddApplicationType_BrowserBased(
            CatalogueItemId catalogueItemId,
            ApplicationTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedApplicationType = ApplicationType.BrowserBased;

            var result = (await controller.AddApplicationType(catalogueItemId, model)).As<RedirectToActionResult>();

            result.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            result.ControllerName.Should().Be(typeof(BrowserBasedController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddApplicationType_Desktop(
            CatalogueItemId catalogueItemId,
            ApplicationTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedApplicationType = ApplicationType.Desktop;

            var result = (await controller.AddApplicationType(catalogueItemId, model)).As<RedirectToActionResult>();

            result.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            result.ControllerName.Should().Be(typeof(DesktopBasedController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddApplicationType_MobileTablet(
            CatalogueItemId catalogueItemId,
            ApplicationTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedApplicationType = ApplicationType.MobileTablet;

            var result = (await controller.AddApplicationType(catalogueItemId, model)).As<RedirectToActionResult>();

            result.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            result.ControllerName.Should().Be(typeof(MobileTabletBasedController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            List<CatalogueItem> solutions,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockSolutionsService
                .GetAllSolutionsForSearchTerm(searchTerm)
                .Returns(solutions);

            var result = await controller.SearchResults(searchTerm);

            await mockSolutionsService
                .Received()
                .GetAllSolutionsForSearchTerm(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            foreach (var solution in solutions)
            {
                actualResult.Should().Contain(x => x.Title == solution.Name && x.Category == solution.Supplier.Name);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockSolutionsService
                .GetAllSolutionsForSearchTerm(searchTerm)
                .Returns(new List<CatalogueItem>());

            var result = await controller.SearchResults(searchTerm);

            await mockSolutionsService
                .Received()
                .GetAllSolutionsForSearchTerm(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_SearchResults_InvalidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            CatalogueSolutionsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(new CatalogueItem());

            await controller.Features(catalogueItemId);

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var expected = new FeaturesModel(catalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(default(CatalogueItem));

            var actual = (await controller.Features(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Features_CallsSavesSolutionFeatures(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Features(catalogueItemId, model);

            await mockService
                .Received()
                .SaveSolutionFeatures(catalogueItemId, Arg.Is<string[]>(a => a.SequenceEqual(model.AllFeatures)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Features_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Features(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var expected = new FeaturesModel(catalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedModel(
            List<CatalogueItem> solutions,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();

            mockService
                .GetAllSolutions(null)
                .Returns(solutions);

            var actual = (await controller.Index()).As<ViewResult>();

            await mockService
                .Received()
                .GetAllSolutions(null);

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            var model = actual.Model.As<CatalogueSolutionsModel>();

            model.Solutions.Should().BeEquivalentTo(expected);
            model.SelectedPublicationStatus.Should().BeNullOrWhiteSpace();
            model.SearchTerm.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_WithSearchTerm_ReturnsViewWithExpectedModel(
            string searchTerm,
            List<CatalogueItem> solutions,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(x => new CatalogueModel(x));

            mockService
                .GetAllSolutionsForSearchTerm(searchTerm)
                .Returns(solutions);

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            await mockService
                .Received()
                .GetAllSolutionsForSearchTerm(searchTerm);

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            var model = actual.Model.As<CatalogueSolutionsModel>();

            model.Solutions.Should().BeEquivalentTo(expected);
            model.SelectedPublicationStatus.Should().BeNullOrWhiteSpace();
            model.SearchTerm.Should().Be(searchTerm);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_StatusInput_SetsSelectedOnModel_ReturnsViewWithModel(
            List<CatalogueItem> solutions,
            PublicationStatus status,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var model = new CatalogueSolutionsModel { SelectedPublicationStatus = status.ToString() };

            mockService
                .GetAllSolutions(status)
                .Returns(solutions);

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            model.SetSolutions(solutions);

            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_StatusInput_WithSearchTerm_FiltersIntersectionOfBothSets_ReturnsViewWithModel(
            List<CatalogueItem> solutions,
            PublicationStatus status,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.First();
            var searchTerm = expected.Name;
            var model = new CatalogueSolutionsModel
            {
                SearchTerm = searchTerm,
                SelectedPublicationStatus = status.ToString(),
            };

            model.SetSolutions(new[] { expected });

            mockService
                .GetAllSolutions(status)
                .Returns(solutions);

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            model.SetSolutions(solutions);

            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ManageSolution_ReturnsViewWithExpectedModel(
            [Frozen] AspNetUser aspNetUser,
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses,
            int userId,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.LastUpdatedBy = userId;
            solution.LastUpdatedByUser = aspNetUser;
            aspNetUser.Id = userId;
            var expectedModel = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            mockSolutionService.GetSolutionThin(solution.CatalogueItemId)
                .Returns(solution.CatalogueItem);

            mockSolutionService.GetSolutionLoadingStatuses(solution.CatalogueItemId)
                .Returns(solutionLoadingStatuses);

            var actual = (await controller.ManageCatalogueSolution(solution.CatalogueItemId)).As<ViewResult>();

            await mockSolutionService
                .Received()
                .GetSolutionThin(solution.CatalogueItemId);

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<ManageCatalogueSolutionModel>();
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Description_ReturnsViewWithExpectedModel(
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var expected = new CatalogueItem
            {
                Id = new CatalogueItemId(999999, "999"),
                Solution = new Solution
                {
                    Summary = "XYZ Summary",
                    FullDescription = "XYZ description",
                    AboutUrl = "Fake url",
                },
                Name = "Fake Solution",
            };
            mockSolutionService.GetSolutionThin(Arg.Any<CatalogueItemId>())
                .Returns(expected);

            var actual = (await controller.Description(expected.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<DescriptionModel>();
            model.SolutionName.Should().BeEquivalentTo("Fake Solution");
            model.Summary.Should().BeEquivalentTo("XYZ Summary");
            model.Description.Should().BeEquivalentTo("XYZ description");
            model.Link.Should().BeEquivalentTo("Fake url");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Details_BadRequest(
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller,
            CatalogueItemId solutionId)
        {
            mockSolutionService.GetSolutionWithBasicInformation(solutionId)
                .Returns((CatalogueItem)null);

            var actual = (await controller.Details(solutionId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Details_WhenNoFrameworkSolutions_ReturnsViewWithExpectedModel(
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] ISuppliersService mockSuppliersService,
            CatalogueSolutionsController controller,
            string shortName,
            string frameworkId,
            CatalogueItemId solutionId,
            CatalogueItem catalogueItem,
            Solution solution,
            List<Supplier> suppliers)
        {
            catalogueItem.Id = solutionId;
            solution.FrameworkSolutions.Clear();
            solution.IsPilotSolution = true;
            catalogueItem.Solution = solution;

            mockSolutionService.GetSolutionWithBasicInformation(solutionId)
                .Returns(catalogueItem);

            mockSuppliersService.GetAllActiveSuppliers()
                .Returns(suppliers);

            var framework = new EntityFramework.Catalogue.Models.Framework
            {
                Id = frameworkId,
                ShortName = shortName,
            };

            mockSolutionService.GetAllFrameworks()
                .Returns(new List<EntityFramework.Catalogue.Models.Framework> { framework });

            var actual = (await controller.Details(solutionId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Details_WithFrameworkSolutions_ReturnsViewWithExpectedModel(
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] ISuppliersService mockSuppliersService,
            CatalogueSolutionsController controller,
            string shortName,
            string frameworkId,
            CatalogueItemId solutionId,
            CatalogueItem catalogueItem,
            Solution solution,
            List<Supplier> suppliers)
        {
            catalogueItem.Id = solutionId;
            solution.FrameworkSolutions.Clear();
            solution.FrameworkSolutions.Add(
                new FrameworkSolution
                {
                    FrameworkId = frameworkId,
                });

            solution.IsPilotSolution = true;
            catalogueItem.Solution = solution;

            mockSolutionService.GetSolutionWithBasicInformation(solutionId)
                .Returns(catalogueItem);

            mockSuppliersService.GetAllActiveSuppliers()
                .Returns(suppliers);

            var framework = new EntityFramework.Catalogue.Models.Framework
            {
                Id = frameworkId,
                ShortName = shortName,
            };

            mockSolutionService.GetAllFrameworks()
                .Returns(new List<EntityFramework.Catalogue.Models.Framework> { framework });

            var actual = (await controller.Details(solutionId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Details_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            SolutionModel model,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionByName(Arg.Any<string>()).Returns(Task.FromResult(new CatalogueItem { Id = catalogueItemId }));

            var actual = (await controller.Details(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDetails_InvalidModel_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            CatalogueSolutionsController controller)
        {
            var solutionModel = new SolutionModel();
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Details(id, solutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(solutionModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDetails_ValidModel_CallsSaveSolutionDescriptionOnService(
            [Frozen] CatalogueItemId id,
            SolutionModel model,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            await controller.Details(id, model);

            await mockSolutionService
                .Received()
                .SaveSolutionDetails(id, model.SolutionName, model.SupplierId ?? default, model.IsPilotSolution, model.Frameworks);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDetails_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            SolutionModel model,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            mockSolutionService.GetSolutionByName(Arg.Any<string>()).Returns(Task.FromResult(new CatalogueItem { Id = id }));

            var actual = (await controller.Details(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Description_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            DescriptionModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Description(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService(
            CatalogueItemId id,
            ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Description(id, new DescriptionModel());

            await mockSolutionService
                .Received(0)
                .SaveSolutionDescription(Arg.Any<CatalogueItemId>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDescription_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId id,
            DescriptionModel editDescriptionModel,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Description(id, editDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(editDescriptionModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            CatalogueItemId id,
            DescriptionModel model,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            await controller.Description(id, model);

            await mockSolutionService
                .Received()
                .SaveSolutionDescription(id, model.Summary, model.Description, model.Link);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDescription_ValidModel_RedirectsToExpectedAction(
            DescriptionModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Description(model.SolutionId!.Value, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Implementation_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(new CatalogueItem());

            await controller.Implementation(catalogueItemId);

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Implementation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(default(CatalogueItem));

            var actual = (await controller.Implementation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Implementation_CallsSaveImplementationDetail(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Implementation(catalogueItemId, model);

            await mockService
                .Received()
                .SaveImplementationDetail(catalogueItemId, Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Implementation_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Implementation(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationType_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController catalogueSolutionsController)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItem.Id)
                .Returns(catalogueItem);

            await catalogueSolutionsController.ApplicationType(catalogueItem.Id);

            await mockService
                .Received()
                .GetSolutionThin(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationType_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var actual = (await controller.ApplicationType(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(default(CatalogueItem));

            var actual = (await controller.ApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ApplicationType_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var actual = (await controller.ApplicationType(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddApplicationType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(solution.CatalogueItem);

            await controller.AddApplicationType(catalogueItemId);

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddApplicationType_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(catalogueItem);

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<ViewResult>();

            await mockService
                .Received()
                .GetSolutionThin(catalogueItemId);

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSelectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            CatalogueSolutionsController controller)
        {
            mockService.GetSolutionThin(catalogueItemId)
                .Returns(default(CatalogueItem));

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_SameStatus_ReturnsRedirectResult(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionPublicationStatusService publicationStatusService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = catalogueItem.PublishedStatus };

            solutionsService.GetSolutionThin(catalogueItem.Id)
                .Returns(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel)).As<RedirectToActionResult>();

            await publicationStatusService
                .Received(0)
                .SetPublicationStatus(Arg.Any<CatalogueItemId>(), Arg.Any<PublicationStatus>());

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            Solution solution,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] ISolutionPublicationStatusService mockPublicationStatusService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.GetSolutionThin(catalogueItem.Id)
                .Returns(catalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel);

            await mockPublicationStatusService
                .Received()
                .SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel.SelectedPublicationStatus);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            Solution solution,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.GetSolutionThin(catalogueItem.Id)
                .Returns(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            CatalogueItem solution,
            PublicationStatus publicationStatus,
            [Frozen] ISolutionsService mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = publicationStatus };

            mockSolutionService.GetSolutionThin(solution.Id)
                .Returns(solution);

            var actual = (await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.Model.Should().BeEquivalentTo(manageCatalogueSolutionModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierDetails_ReturnsModel(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id)
                .Returns(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id)).As<ViewResult>();
            var model = result.Model.As<EditSolutionContactsModel>();

            result.Should().NotBeNull();
            model.Should().NotBeNull();
            model.SupplierName.Should().Be(catalogueItem.Supplier.Name);
            model.AvailableSupplierContacts.Should().HaveCount(catalogueItem.Supplier.SupplierContacts.Count);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierDetails_InvalidModel_ReturnsViewWithModel(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id)
                .Returns(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierDetails_ValidModel_ReturnsRedirectToActionResult(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id)
                .Returns(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierDetails_ValidModel_SavesContacts(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            var filteredSelectedContacts = model.AvailableSupplierContacts.Where(sc => sc.Selected).ToList();

            var expectedContacts = catalogueItem.Supplier.SupplierContacts.Join(
                filteredSelectedContacts,
                outer => outer.Id,
                inner => inner.Id,
                (supplierContact, _) => supplierContact.Id).OrderBy(i => i);

            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id)
                .Returns(catalogueItem);

            _ = await controller.EditSupplierDetails(catalogueItem.Id, model);

            await solutionsService
                .Received()
                .SaveContacts(
                    catalogueItem.Id,
                    Arg.Is<List<SupplierContact>>(x => x.Select(i => i.Id).OrderBy(i => i).SequenceEqual(expectedContacts)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionWithCapabilities(catalogueItemId)
                .Returns(default(CatalogueItem));

            var result = await controller.EditCapabilities(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_ValidId_ReturnsModel(
            Solution solution,
            IReadOnlyList<CapabilityCategory> capabilityCategories,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ICapabilitiesService capabilitiesService,
            CatalogueSolutionsController controller)
        {
            var expectedModel = new EditCapabilitiesModel(solution.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            capabilitiesService.GetCapabilitiesByCategory()
                .Returns(capabilityCategories.ToList());

            solutionsService.GetSolutionWithCapabilities(solution.CatalogueItemId)
                .Returns(solution.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_InvalidModel_ReturnsViewWithModel(
            Solution solution,
            EditCapabilitiesModel model,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.EditCapabilities(solution.CatalogueItemId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId)
                .Returns(default(CatalogueItem));

            var result = await controller.EditCapabilities(solution.CatalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_ValidModel_AddsCapabilitiesToCatalogueItem(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ICapabilitiesService capabilitiesService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId)
                .Returns(solution.CatalogueItem);

            _ = await controller.EditCapabilities(solution.CatalogueItemId, model);

            await capabilitiesService
                .Received()
                .AddCapabilitiesToCatalogueItem(solution.CatalogueItemId, Arg.Any<SaveCatalogueItemCapabilitiesModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToManageCatalogueSolution(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] ISolutionsService solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId)
                .Returns(solution.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            result.As<RedirectToActionResult>().RouteValues.Should().Contain(
                new KeyValuePair<string, object>("solutionId", solution.CatalogueItemId));
        }
    }
}
