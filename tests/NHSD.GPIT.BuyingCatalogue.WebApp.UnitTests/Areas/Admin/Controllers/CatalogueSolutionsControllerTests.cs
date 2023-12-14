using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_ReturnsExpectedResult(
            string searchTerm,
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockSolutionsService
                .Setup(o => o.GetAllSolutionsForSearchTerm(searchTerm))
                .ReturnsAsync(solutions);

            var result = await controller.SearchResults(searchTerm);

            mockSolutionsService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            foreach (var solution in solutions)
            {
                actualResult.Should().Contain(x => x.Title == solution.Name && x.Category == solution.Supplier.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_ValidSearchTerm_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockSolutionsService
                .Setup(o => o.GetAllSolutionsForSearchTerm(searchTerm))
                .ReturnsAsync(new List<CatalogueItem>());

            var result = await controller.SearchResults(searchTerm);

            mockSolutionsService.VerifyAll();

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
            CatalogueSolutionsController controller)
        {
            var result = await controller.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.Features(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var expected = new FeaturesModel(catalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Features(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_CallsSavesSolutionFeatures(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Features(catalogueItemId, model);

            mockService.Verify(s => s.SaveSolutionFeatures(catalogueItemId, model.AllFeatures));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var expected = new FeaturesModel(catalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedModel(
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();

            mockService
                .Setup(s => s.GetAllSolutions(null))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index()).As<ViewResult>();

            mockService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            var model = actual.Model.As<CatalogueSolutionsModel>();

            model.Solutions.Should().BeEquivalentTo(expected);
            model.SelectedPublicationStatus.Should().BeNullOrWhiteSpace();
            model.SearchTerm.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_WithSearchTerm_ReturnsViewWithExpectedModel(
            string searchTerm,
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(x => new CatalogueModel(x));

            mockService
                .Setup(s => s.GetAllSolutionsForSearchTerm(searchTerm))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index(searchTerm)).As<ViewResult>();

            mockService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            var model = actual.Model.As<CatalogueSolutionsModel>();

            model.Solutions.Should().BeEquivalentTo(expected);
            model.SelectedPublicationStatus.Should().BeNullOrWhiteSpace();
            model.SearchTerm.Should().Be(searchTerm);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_StatusInput_SetsSelectedOnModel_ReturnsViewWithModel(
            List<CatalogueItem> solutions,
            PublicationStatus status,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var model = new CatalogueSolutionsModel { SelectedPublicationStatus = status.ToString() };

            mockService
                .Setup(s => s.GetAllSolutions(status))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            model.SetSolutions(solutions);

            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_StatusInput_WithSearchTerm_FiltersIntersectionOfBothSets_ReturnsViewWithModel(
            List<CatalogueItem> solutions,
            PublicationStatus status,
            [Frozen] Mock<ISolutionsService> mockService,
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
                .Setup(s => s.GetAllSolutions(status))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            model.SetSolutions(solutions);

            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageSolution_ReturnsViewWithExpectedModel(
            [Frozen] AspNetUser aspNetUser,
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses,
            int userId,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.LastUpdatedBy = userId;
            solution.LastUpdatedByUser = aspNetUser;
            aspNetUser.Id = userId;
            var expectedModel = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            mockSolutionService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockSolutionService.Setup(s => s.GetSolutionLoadingStatuses(solution.CatalogueItemId))
                .ReturnsAsync(solutionLoadingStatuses);

            var actual = (await controller.ManageCatalogueSolution(solution.CatalogueItemId)).As<ViewResult>();

            mockSolutionService.Verify(s => s.GetSolutionThin(solution.CatalogueItemId));
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<ManageCatalogueSolutionModel>();
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ReturnsViewWithExpectedModel(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
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
            mockSolutionService.Setup(s => s.GetSolutionThin(It.IsAny<CatalogueItemId>()))
                .ReturnsAsync(expected);

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
        [CommonAutoData]
        public static async Task Get_Details_ReturnsViewWithExpectedModel(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            CatalogueSolutionsController controller,
            string shortName,
            string frameworkId,
            bool selected,
            bool isFoundation,
            bool supportsFoundationSolution,
            CatalogueItemId solutionId,
            CatalogueItem catalogueItem,
            Solution solution,
            List<Supplier> suppliers)
        {
            var frameworkModel = new FrameworkModel
            {
                Name = $"{shortName} Framework",
                FrameworkId = frameworkId,
                Selected = selected,
                IsFoundation = isFoundation,
                SupportsFoundationSolution = supportsFoundationSolution,
            };
            var expected = new SolutionModel
            {
                Frameworks = new List<FrameworkModel> { frameworkModel },
                SolutionId = solutionId,
            };

            catalogueItem.Id = solutionId;
            solution.FrameworkSolutions.Clear();
            solution.FrameworkSolutions.Add(new FrameworkSolution { IsFoundation = isFoundation });
            solution.IsPilotSolution = true;
            catalogueItem.Solution = solution;

            mockSolutionService.Setup(s => s.GetSolutionWithBasicInformation(solutionId))
                .ReturnsAsync(catalogueItem);

            mockSuppliersService.Setup(s => s.GetAllActiveSuppliers())
                .ReturnsAsync(suppliers);

            var framework = new EntityFramework.Catalogue.Models.Framework
            {
                Id = frameworkId,
                ShortName = shortName,
                SupportsFoundationSolution = supportsFoundationSolution,
            };

            mockSolutionService.Setup(s => s.GetAllFrameworks())
                .ReturnsAsync(new List<EntityFramework.Catalogue.Models.Framework> { framework });

            var actual = (await controller.Details(solutionId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<SolutionModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Details_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            SolutionModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionByName(It.IsAny<string>())).Returns(Task.FromResult(new CatalogueItem { Id = catalogueItemId }));

            var actual = (await controller.Details(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditDetails_ValidModel_CallsSaveSolutionDescriptionOnService(
            [Frozen] CatalogueItemId id,
            SolutionModel model,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            mockSolutionService.Setup(m => m.GetSolutionByName(It.IsAny<string>())).Returns(Task.FromResult(new CatalogueItem { Id = id }));

            await controller.Details(id, model);

            mockSolutionService.Verify(s => s.SaveSolutionDetails(id, model.SolutionName, model.SupplierId ?? default, model.IsPilotSolution, model.Frameworks));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDetails_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            SolutionModel model,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            mockSolutionService.Setup(m => m.GetSolutionByName(It.IsAny<string>())).Returns(Task.FromResult(new CatalogueItem { Id = id }));

            var actual = (await controller.Details(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService(
            CatalogueItemId id,
            Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Description(id, new Mock<DescriptionModel>().Object);

            mockSolutionService.Verify(
                s => s.SaveSolutionDescription(It.IsAny<CatalogueItemId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            CatalogueItemId id,
            DescriptionModel model,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            await controller.Description(id, model);

            mockSolutionService.Verify(s => s.SaveSolutionDescription(id, model.Summary, model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Get_Implementation_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.Implementation(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Implementation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_CallsSaveImplementationDetail(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Implementation(catalogueItemId, model);

            mockService.Verify(s => s.SaveImplementationDetail(catalogueItemId, It.IsAny<string>()));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ApplicationType_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController catalogueSolutionsController)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            await catalogueSolutionsController.ApplicationType(catalogueItem.Id);

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ApplicationType_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ApplicationType_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.AddApplicationType(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ApplicationTypeSelectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_SameStatus_ReturnsRedirectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<ISolutionPublicationStatusService> publicationStatusService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = catalogueItem.PublishedStatus };

            solutionsService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel)).As<RedirectToActionResult>();

            publicationStatusService.VerifyNoOtherCalls();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<ISolutionPublicationStatusService> mockPublicationStatusService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel);

            mockPublicationStatusService.Verify(s => s.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel.SelectedPublicationStatus));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, manageCatalogueSolutionModel)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            CatalogueItem solution,
            PublicationStatus publicationStatus,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = publicationStatus };

            mockSolutionService.Setup(s => s.GetSolutionThin(solution.Id))
                .ReturnsAsync(solution);

            var actual = (await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.Model.Should().BeEquivalentTo(manageCatalogueSolutionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierDetails_ReturnsModel(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionWithSupplierDetails(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id)).As<ViewResult>();
            var model = result.Model.As<EditSolutionContactsModel>();

            result.Should().NotBeNull();
            model.Should().NotBeNull();
            model.SupplierName.Should().Be(catalogueItem.Supplier.Name);
            model.AvailableSupplierContacts.Should().HaveCount(catalogueItem.Supplier.SupplierContacts.Count);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDetails_InvalidModel_ReturnsViewWithModel(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            solutionsService.Setup(s => s.GetSolutionWithSupplierDetails(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDetails_ValidModel_ReturnsRedirectToActionResult(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionWithSupplierDetails(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = (await controller.EditSupplierDetails(catalogueItem.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDetails_ValidModel_SavesContacts(
            EditSolutionContactsModel model,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            var filteredSelectedContacts = model.AvailableSupplierContacts.Where(sc => sc.Selected).ToList();
            var expectedContacts = catalogueItem.Supplier.SupplierContacts.Join(
                filteredSelectedContacts,
                outer => outer.Id,
                inner => inner.Id,
                (supplierContact, _) => supplierContact).ToList();

            solutionsService.Setup(s => s.GetSolutionWithSupplierDetails(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            _ = await controller.EditSupplierDetails(catalogueItem.Id, model);

            solutionsService.Verify(s => s.SaveContacts(catalogueItem.Id, expectedContacts));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionWithCapabilities(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditCapabilities(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_ValidId_ReturnsModel(
            Solution solution,
            IReadOnlyList<CapabilityCategory> capabilityCategories,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            CatalogueSolutionsController controller)
        {
            var expectedModel = new EditCapabilitiesModel(solution.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            capabilitiesService.Setup(s => s.GetCapabilitiesByCategory())
                .ReturnsAsync(capabilityCategories.ToList());

            solutionsService.Setup(s => s.GetSolutionWithCapabilities(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditCapabilities(solution.CatalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_ValidModel_AddsCapabilitiesToCatalogueItem(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            _ = await controller.EditCapabilities(solution.CatalogueItemId, model);

            capabilitiesService.Verify(s => s.AddCapabilitiesToCatalogueItem(solution.CatalogueItemId, It.IsAny<SaveCatalogueItemCapabilitiesModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToManageCatalogueSolution(
            Solution solution,
            EditCapabilitiesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            CatalogueSolutionsController controller)
        {
            solutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            result.As<RedirectToActionResult>().RouteValues.Should().Contain(
                new KeyValuePair<string, object>("solutionId", solution.CatalogueItemId));
        }
    }
}
