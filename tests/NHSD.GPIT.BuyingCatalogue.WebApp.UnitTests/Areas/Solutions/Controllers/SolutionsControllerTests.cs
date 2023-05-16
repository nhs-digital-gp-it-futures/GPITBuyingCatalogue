using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class SolutionsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_NoSearch_GetsSolutionFromService(
            Solution solution,
            PageOptions options,
            [Frozen] Mock<ISolutionsFilterService> mockService,
            SolutionsController controller)
        {
            var itemsToReturn = new List<CatalogueItem>() { solution.CatalogueItem };

            mockService.Setup(s => s.GetAllSolutionsFiltered(It.IsAny<PageOptions>(), null, null, null, null, null))
                .ReturnsAsync((itemsToReturn, options, new List<CapabilitiesAndCountModel>()));

            await controller.Index(options.PageNumber.ToString(), options.Sort.ToString(), null, null, null, null, null, null);

            mockService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_Redirects(
            SolutionsModel solutionModel,
            AdditionalFiltersModel additionalFilters,
            SolutionsController controller)
        {
            var result = controller.Index(solutionModel, null, null, null, null, null, null, additionalFilters);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "page", null },
                { "sortBy", solutionModel.SelectedSortOption.ToString() },
                { "search", null },
                { "selectedCapabilityIds", null },
                { "selectedEpicIds", null },
                { "selectedFrameworkId", null },
                { "selectedClientApplicationTypeIds", additionalFilters.CombineSelectedOptions(additionalFilters.ClientApplicationTypeOptions) },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async void GetFilterSearchSuggestions_ReturnsJsonResult(
            string search,
            string currentPage,
            List<SearchFilterModel> searchResults,
            [Frozen] Mock<ISolutionsFilterService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionsBySearchTerm(search, It.IsAny<int>()))
                .ReturnsAsync(searchResults);

            var context = new DefaultHttpContext();
            context.HttpContext.Request.Headers.Referer = currentPage;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context,
            };

            var currentPageUrl = new UriBuilder(currentPage);
            var expectedResults = searchResults.Select(r =>
                new SuggestionSearchResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).ToString(),
                });

            var result = await controller.FilterSearchSuggestions(search);

            mockService.VerifyAll();
            var actualResult = result.Should().BeOfType<JsonResult>().Subject;
            actualResult.Value.Should().BeEquivalentTo(expectedResults);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            mockService.Setup(s => s.GetPublishedAssociatedServicesForSolution(id))
                .ReturnsAsync(associatedServices);

            await controller.AssociatedServices(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
            mockService.Verify(s => s.GetPublishedAssociatedServicesForSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AssociatedServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            ClientApplication clientApplication,
            Solution solution,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.ClientApplication = JsonSerializer.Serialize(clientApplication);
            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();
            var associatedServicesModel = new AssociatedServicesModel(catalogueItem, associatedServices, contentStatus);

            solutionsServiceMock.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            solutionsServiceMock.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            solutionsServiceMock.Setup(s => s.GetPublishedAssociatedServicesForSolution(catalogueItem.Id))
                .ReturnsAsync(associatedServices);

            var actual = (await controller.AssociatedServices(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(associatedServicesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionWithCapabilities(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.Capabilities(id);

            mockService.Verify(s => s.GetSolutionWithCapabilities(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionWithCapabilities(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Capabilities(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            var capabilitiesViewModel = new CapabilitiesViewModel(catalogueItem, contentStatus);
            mockService.Setup(s => s.GetSolutionWithCapabilities(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Capabilities(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(capabilitiesViewModel, opt => opt.Excluding(cvm => cvm.BackLink).Excluding(cvm => cvm.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullCatalogueItem_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            mockService.VerifyAll();
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullSolution_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(new CatalogueItem() { Solution = null });

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            mockService.VerifyAll();
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_PublicationSuspended_ReturnsRedirectToAction(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Suspended };

            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<RedirectToActionResult>();

            mockService.VerifyAll();
            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionsController.Description));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NoCatalogueItemForAdditionalService_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published };

            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServices.Setup(s => s.GetAdditionalServiceWithCapabilities(additionalServiceId))
                .ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            mockService.VerifyAll();
            mockAdditionalServices.VerifyAll();
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullAdditionalService_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published };

            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServices.Setup(s => s.GetAdditionalServiceWithCapabilities(additionalServiceId))
                .ReturnsAsync(new CatalogueItem() { AdditionalService = null });

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            mockService.VerifyAll();
            mockAdditionalServices.VerifyAll();
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_ReturnsDefaultView(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution,
            AdditionalService additionalService,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published };

            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItemId))
                .ReturnsAsync(contentStatus);

            var additionalCatalogueItem = new CatalogueItem() { AdditionalService = additionalService };
            mockAdditionalServices.Setup(s => s.GetAdditionalServiceWithCapabilities(additionalServiceId))
                .ReturnsAsync(additionalCatalogueItem);

            var expectedModel = new CapabilitiesViewModel(catalogueItem, additionalCatalogueItem, contentStatus)
                { Name = additionalCatalogueItem.Name, Description = additionalService.FullDescription, };

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<ViewResult>();

            mockService.VerifyAll();
            mockAdditionalServices.VerifyAll();
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(cvm => cvm.BackLink).Excluding(cvm => cvm.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Standards_GetSolutionAndStandardsFromService(
            Solution solution,
            List<Standard> standards,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            mockService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            mockService.Setup(s => s.GetWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(new List<WorkOffPlan>());

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            await controller.Standards(solution.CatalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(solution.CatalogueItemId));
            mockService.Verify(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId));
            mockService.Verify(s => s.GetWorkOffPlans(solution.CatalogueItemId));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Standards_SolutionDoesNotExist_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Standards(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Standards_ValidSolutionForId_ReturnsExpectedResultView(
            Solution solution,
            List<Standard> standards,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var solutionStandardsModel = new SolutionStandardsModel(
                solution.CatalogueItem,
                standards,
                new List<string>(),
                contentStatus);

            mockService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            mockService.Setup(s => s.GetWorkOffPlans(solution.CatalogueItemId)).ReturnsAsync(new List<WorkOffPlan>());

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Standards(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(solutionStandardsModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ClientApplicationTypes(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionForId_ReturnsExpectedViewResult(
            CatalogueItemId id,
            [Frozen] ClientApplication clientApplication,
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.ClientApplication = JsonSerializer.Serialize(clientApplication);

            var expectedModel = new ClientApplicationTypesModel(catalogueItem, contentStatus);
            solutionsService.Setup(s => s.GetSolutionThin(id)).ReturnsAsync(catalogueItem);

            solutionsService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.ClientApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_ValidId_ResultAsExpected(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            var catalogueItemId = solution.Id;
            var capabilityItem = solution.CatalogueItemCapabilities.First();
            var capabilityId = capabilityItem.Capability.Id;
            var capability = solution.CatalogueItemCapabilities.First();

            var expectedModel = new SolutionCheckEpicsModel(capability, solution);

            mockSolutionsService
                .Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(solution);

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(em => em.BackLinkText).Excluding(em => em.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            int capabilityId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should()
                .Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
        }

        [Fact]
        public static void Get_CheckEpics_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.CheckEpics))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be("{solutionId}/capability/{capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_ValidIds_ResultAsExpected(
            AdditionalService additionalSolution,
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            var catalogueItemIdAdditional = additionalSolution.CatalogueItemId;
            var capabilityItem = additionalSolution.CatalogueItem.CatalogueItemCapabilities.First();
            var capabilityId = capabilityItem.Capability.Id;
            var capability = additionalSolution.CatalogueItem.CatalogueItemCapabilities.First();

            var expectedModel = new SolutionCheckEpicsModel(capability, additionalSolution.CatalogueItem, additionalSolution.CatalogueItemId);

            mockSolutionsService
                .Setup(s => s.GetSolutionCapability(additionalSolution.CatalogueItemId, capabilityId))
                    .ReturnsAsync(additionalSolution.CatalogueItem);

            mockSolutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.CheckEpicsAdditionalServices(
                    solution.CatalogueItemId,
                    catalogueItemIdAdditional,
                    capabilityId))
                .As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("CheckEpics");
            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(em => em.BackLink).Excluding(em => em.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            AdditionalService additionalSolution,
            Solution solution,
            int capabilityId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetSolutionCapability(additionalSolution.CatalogueItemId, capabilityId))
                    .ReturnsAsync(default(CatalogueItem));

            mockSolutionsService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller
                    .CheckEpicsAdditionalServices(solution.CatalogueItemId, additionalSolution.CatalogueItemId, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalSolution.CatalogueItemId} with Capability Id: {capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_NullSolution_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            int capabilityId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller
                    .CheckEpicsAdditionalServices(catalogueItemId, additionalServiceId, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId}");
        }

        [Fact]
        public static void Get_CheckEpicsAdditionalServices_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.CheckEpicsAdditionalServices))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be(
                    "{solutionId}/additional-services/{additionalServiceId}/capability/{capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidId_GetsSolutionFromService(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            mockService.Setup(s => s.GetSolutionWithBasicInformation(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            await controller.Description(solution.CatalogueItemId);

            mockService.Verify(s => s.GetSolutionWithBasicInformation(solution.CatalogueItemId));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithBasicInformation(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Description(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var solutionDescriptionModel = new SolutionDescriptionModel(catalogueItem, contentStatus);

            mockService.Setup(s => s.GetSolutionWithBasicInformation(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Description(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(solutionDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_InvokesGetSolutionOverview(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            await controller.Features(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Features(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(new SolutionFeaturesModel(catalogueItem, contentStatus));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.HostingType(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HostingType(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var expectedModel = new HostingTypesModel(catalogueItem, contentStatus);
            solutionsServiceMock.Setup(s => s.GetSolutionThin(solution.CatalogueItemId)).ReturnsAsync(catalogueItem);

            solutionsServiceMock.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.HostingType(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(
            [Frozen] Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionThin(id)).ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.Implementation(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Implementation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var implementationTimescalesModel = new ImplementationTimescalesModel(catalogueItem, contentStatus);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Implementation(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(implementationTimescalesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_ValidId_GetsSolutionFromService(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionWithCataloguePrice(It.IsAny<CatalogueItemId>()))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            await controller.ListPrice(catalogueItem.Id);

            mockService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithCataloguePrice(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ListPrice(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var mockSolutionListPriceModel = new ListPriceModel(catalogueItem, contentStatus);

            mockService.Setup(s => s.GetSolutionWithCataloguePrice(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.ListPrice(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(mockSolutionListPriceModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.Interoperability(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Interoperability(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus,
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.Integrations = GetIntegrationsJson();

            var expectedViewData = new InteroperabilityModel(catalogueItem, contentStatus);

            mockSolutionService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockSolutionService.Setup(s => s.GetContentStatusForCatalogueItem(catalogueItem.Id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.Interoperability(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionWithSupplierDetails(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            await controller.SupplierDetails(id);

            mockService.Verify(s => s.GetSolutionWithSupplierDetails(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithSupplierDetails(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.SupplierDetails(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var expectedSolutionSupplierDetailsModel = new SolutionSupplierDetailsModel(catalogueItem, contentStatus);

            mockService.Setup(s => s.GetSolutionWithSupplierDetails(id))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.SupplierDetails(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedSolutionSupplierDetailsModel);
        }

        [Fact]
        public static void Get_AdditionalServices_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.AdditionalServices))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be("{solutionId}/additional-services");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidId_InvokesGetSolution(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var additionalServices = solution.AdditionalServices.Select(add => add.CatalogueItem).ToList();

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            mockService.Setup(s => s.GetPublishedAdditionalServicesForSolution(id))
                .ReturnsAsync(additionalServices);

            await controller.AdditionalServices(id);

            mockService.Verify(s => s.GetSolutionThin(id));
            mockService.Verify(s => s.GetContentStatusForCatalogueItem(id));
            mockService.Verify(s => s.GetPublishedAdditionalServicesForSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var additionalServices = solution.AdditionalServices.Select(add => add.CatalogueItem).ToList();
            var expectedAdditionalServicesModel =
                new AdditionalServicesModel(solution.CatalogueItem, additionalServices, contentStatus);

            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(id))
                .ReturnsAsync(contentStatus);

            mockService.Setup(s => s.GetPublishedAdditionalServicesForSolution(id))
                .ReturnsAsync(additionalServices);

            var actual = (await controller.AdditionalServices(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedAdditionalServicesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ServiceLevelAgreement_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithServiceLevelAgreements(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ServiceLevelAgreement(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ServiceLevelAgreement_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var expectedModel = new ServiceLevelAgreementDetailsModel(solution.CatalogueItem, contentStatus);

            mockService.Setup(s => s.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            var actual = (await controller.ServiceLevelAgreement(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DevelopmentPlans_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionThin(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DevelopmentPlans_ValidSolutionForId_ReturnsExpectViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var expectedModel = new DevelopmentPlansModel(solution.CatalogueItem, new List<WorkOffPlan>(), contentStatus);

            mockService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetContentStatusForCatalogueItem(solution.CatalogueItemId))
                .ReturnsAsync(contentStatus);

            mockService.Setup(s => s.GetWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(new List<WorkOffPlan>());

            var actual = (await controller.DevelopmentPlans(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_AboutPilotSolutions_ReturnsViewWithModel(
            SolutionsController controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Headers.Referer = "http://test.com";

            var result = controller.AboutPilotSolutions().As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<NavBaseModel>();
            result.Model.As<NavBaseModel>().BackLink.Should().Be(controller.Request.Headers.Referer);
        }

        private static string GetIntegrationsJson()
        {
            var integrations = new List<Integration>
            {
                new Integration
                {
                    Id = Guid.NewGuid(),
                    IntegrationType = "IM1",
                    Qualifier = "Bulk",
                    IntegratesWith = "Audit+",
                    Description = "Audit+ utilises a bulk extraction of full clinical records (including confidential and deceased patients) from EMIS Web to provide General Practices with a crossplatform clinical decision support and management tool; supporting QOF performance management, improvement and NHS Health Checks.",
                },
                new Integration
                {
                    Id = Guid.NewGuid(),
                    IntegrationType = "GP Connect",
                    Qualifier = "Access Record HTML",
                    AdditionalInformation = "EMIS Web received Full Roll Out Approval from NHS Digital for GP Connect HTML View Provision on 20/06/19",
                },
            };

            return JsonSerializer.Serialize(integrations);
        }
    }
}
