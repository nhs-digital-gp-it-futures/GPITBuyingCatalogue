﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class SolutionsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_NoSearch_GetsSolutionFromService(
            Solution solution,
            PageOptions options,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] ISolutionsFilterService mockService,
            [Frozen] IFrameworkService mockFrameworkService,
            SolutionsController controller)
        {
            var itemsToReturn = new List<CatalogueItem>() { solution.CatalogueItem };
            var capabilitiesAndEpics = new Dictionary<int, string[]>();

            mockFrameworkService.GetFrameworksWithPublishedCatalogueItems()
                .Returns(frameworks);

            mockService.GetAllSolutionsFiltered(
                    Arg.Any<PageOptions>(),
                    capabilitiesAndEpics)
                .Returns((itemsToReturn, options, new List<CapabilitiesAndCountModel>()));

            var result = await controller.Index(options.PageNumber.ToString(), options.Sort.ToString(), null, null, null, null, null, null);
            result.Should().BeOfType<ViewResult>();

            await mockService.Received()
                .GetAllSolutionsFiltered(
                    Arg.Any<PageOptions>(),
                    Arg.Any<Dictionary<int, string[]>>(),
                    null,
                    null,
                    null,
                    null,
                    Arg.Any<Dictionary<SupportedIntegrations, int[]>>());
        }

        [Theory]
        [MockAutoData]
        public static void Post_Index_Redirects(
            AdditionalFiltersModel additionalFilters,
            SolutionsController controller)
        {
            var result = controller.Index(additionalFilters);

            var selectedInteroperabilityOptions = additionalFilters.GetIntegrationIds();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "sortBy", additionalFilters.SortBy },
                        { "search", null },
                        { "selected", additionalFilters.Selected },
                        { "selectedFrameworkId", additionalFilters.SelectedFrameworkId },
                        {
                            "selectedApplicationTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.ApplicationTypeOptions)
                        },
                        {
                            "selectedHostingTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.HostingTypeOptions)
                        },
                        {
                            "selectedIntegrations",
                            selectedInteroperabilityOptions
                        },
                        {
                            "page",
                            1
                        },
                    });
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectCapabilities_Redirects(
            AdditionalFiltersModel additionalFilters,
            SolutionsController controller)
        {
            var result = controller.SelectCapabilities(additionalFilters);

            var selectedInteroperabilityOptions = additionalFilters.GetIntegrationIds();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterCapabilities));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "sortBy", additionalFilters.SortBy },
                        { "search", null },
                        { "selected", additionalFilters.Selected },
                        { "selectedFrameworkId", additionalFilters.SelectedFrameworkId },
                        {
                            "selectedApplicationTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.ApplicationTypeOptions)
                        },
                        {
                            "selectedHostingTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.HostingTypeOptions)
                        },
                        {
                            "selectedIntegrations",
                            selectedInteroperabilityOptions
                        },
                        {
                            "page",
                            1
                        },
                    });
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectEpics_Redirects(
            AdditionalFiltersModel additionalFilters,
            SolutionsController controller)
        {
            var result = controller.SelectEpics(additionalFilters);

            var selectedInteroperabilityOptions = additionalFilters.GetIntegrationIds();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "sortBy", additionalFilters.SortBy },
                        { "search", null },
                        { "selected", additionalFilters.Selected },
                        { "selectedFrameworkId", additionalFilters.SelectedFrameworkId },
                        {
                            "selectedApplicationTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.ApplicationTypeOptions)
                        },
                        {
                            "selectedHostingTypeIds",
                            additionalFilters.CombineSelectedOptions(additionalFilters.HostingTypeOptions)
                        },
                        {
                            "selectedIntegrations",
                            selectedInteroperabilityOptions
                        },
                        {
                            "page",
                            1
                        },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResult_NoSearch_GetsSolutionFromService(
            Solution solution,
            RequestedFilters filters,
            PageOptions options,
            [Frozen] ISolutionsFilterService mockService,
            SolutionsController controller)
        {
            var itemsToReturn = new List<CatalogueItem>() { solution.CatalogueItem };
            filters = filters with { Selected = string.Empty };

            mockService.GetAllSolutionsFiltered(
                    Arg.Any<PageOptions>(),
                    filters.GetCapabilityAndEpicIds(),
                    filters.Search,
                    filters.SelectedFrameworkId,
                    filters.SelectedApplicationTypeIds,
                    filters.SelectedHostingTypeIds,
                    filters.GetIntegrationsAndTypes())
                .Returns((itemsToReturn, options, new List<CapabilitiesAndCountModel>()));

            var result = await controller.SearchResults(filters);

            result.Should().BeOfType<PartialViewResult>();

            await mockService.Received().GetAllSolutionsFiltered(
                Arg.Any<PageOptions>(),
                Arg.Any<Dictionary<int, string[]>>(),
                filters.Search,
                filters.SelectedFrameworkId,
                filters.SelectedApplicationTypeIds,
                filters.SelectedHostingTypeIds,
                Arg.Any<Dictionary<SupportedIntegrations, int[]>>());
        }

        [Theory]
        [MockAutoData]
        public static async void GetFilterSearchSuggestions_ReturnsJsonResult(
            string search,
            Uri uri,
            List<SearchFilterModel> searchResults,
            [Frozen] ISolutionsFilterService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionsBySearchTerm(search, Arg.Any<int>()).Returns(searchResults);

            var context = new DefaultHttpContext();
            context.HttpContext.Request.Headers.Referer = uri.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context,
            };

            var currentPageUrl = new UriBuilder(uri.ToString());
            var expectedResults = searchResults.Select(r =>
                new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).Uri.PathAndQuery));

            var result = await controller.FilterSearchSuggestions(search);

            await mockService.Received().GetSolutionsBySearchTerm(search, Arg.Any<int>());
            var actualResult = result.Should().BeOfType<JsonResult>().Subject;
            actualResult.Value.Should().BeEquivalentTo(expectedResults);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            mockService.GetPublishedAssociatedServicesForSolution(id).Returns(associatedServices);

            await controller.AssociatedServices(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
            await mockService.Received().GetPublishedAssociatedServicesForSolution(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.AssociatedServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService solutionsServiceMock,
            Solution solution,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();
            var associatedServicesModel = new AssociatedServicesModel(catalogueItem, associatedServices, contentStatus);

            solutionsServiceMock.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            solutionsServiceMock.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            solutionsServiceMock.GetPublishedAssociatedServicesForSolution(catalogueItem.Id).Returns(associatedServices);

            var actual = (await controller.AssociatedServices(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(associatedServicesModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_SuspendedStatus_ReturnsRedirect(
            [Frozen] ISolutionsService solutionsServiceMock,
            Solution solution,
            SolutionsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Suspended;
            var catalogueItem = solution.CatalogueItem;

            solutionsServiceMock.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.AssociatedServices(catalogueItem.Id);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Capabilities_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithCapabilities(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.Capabilities(id);

            await mockService.Received().GetSolutionWithCapabilities(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Capabilities_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithCapabilities(id).Returns(default(CatalogueItem));

            var actual = (await controller.Capabilities(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Capabilities_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var capabilitiesViewModel = new CapabilitiesViewModel(catalogueItem, contentStatus);

            mockService.GetSolutionWithCapabilities(catalogueItem.Id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.Capabilities(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(capabilitiesViewModel, opt => opt.Excluding(cvm => cvm.BackLink).Excluding(cvm => cvm.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullCatalogueItem_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns((CatalogueItem)null);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullSolution_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(new CatalogueItem() { Solution = null });

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_PublicationSuspended_ReturnsRedirectToAction(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Suspended };

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<RedirectToActionResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionsController.Description));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NoCatalogueItemForAdditionalService_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            [Frozen] IAdditionalServicesService mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published };

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            mockAdditionalServices.GetAdditionalServiceWithCapabilities(additionalServiceId).Returns((CatalogueItem)null);

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            await mockAdditionalServices.Received().GetAdditionalServiceWithCapabilities(additionalServiceId);
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_NullAdditionalService_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            [Frozen] IAdditionalServicesService mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published };

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            mockAdditionalServices.GetAdditionalServiceWithCapabilities(additionalServiceId).Returns(new CatalogueItem() { AdditionalService = null });

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            await mockAdditionalServices.Received().GetAdditionalServiceWithCapabilities(additionalServiceId);
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CapabilitiesAdditionalServices_ReturnsDefaultView(
            [Frozen] ISolutionsService mockService,
            [Frozen] IAdditionalServicesService mockAdditionalServices,
            SolutionsController controller,
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            Solution solution,
            Supplier supplier,
            AdditionalService additionalService,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = new CatalogueItem { Solution = solution, PublishedStatus = PublicationStatus.Published, Supplier = supplier };

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(catalogueItemId).Returns(contentStatus);

            var additionalCatalogueItem = new CatalogueItem() { AdditionalService = additionalService };
            mockAdditionalServices.GetAdditionalServiceWithCapabilities(additionalServiceId).Returns(additionalCatalogueItem);

            var expectedModel = new CapabilitiesViewModel(catalogueItem, additionalCatalogueItem, contentStatus)
            { Name = additionalCatalogueItem.Name, Description = additionalService.FullDescription, };

            var actual = (await controller.CapabilitiesAdditionalServices(catalogueItemId, additionalServiceId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            await mockService.Received().GetContentStatusForCatalogueItem(catalogueItemId);
            await mockAdditionalServices.Received().GetAdditionalServiceWithCapabilities(additionalServiceId);
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(cvm => cvm.BackLink).Excluding(cvm => cvm.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Standards_GetSolutionAndStandardsFromService(
            Solution solution,
            List<Standard> standards,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            mockService.GetSolutionWithBasicInformation(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetSolutionStandardsForMarketing(solution.CatalogueItemId).Returns(standards);

            mockService.GetWorkOffPlans(solution.CatalogueItemId).Returns(new List<WorkOffPlan>());

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            await controller.Standards(solution.CatalogueItemId);

            await mockService.Received().GetSolutionWithBasicInformation(solution.CatalogueItemId);
            await mockService.Received().GetSolutionStandardsForMarketing(solution.CatalogueItemId);
            await mockService.Received().GetWorkOffPlans(solution.CatalogueItemId);
            await mockService.Received().GetContentStatusForCatalogueItem(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Standards_SolutionDoesNotExist_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.Standards(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Standards_ValidSolutionForId_ReturnsExpectedResultView(
            Solution solution,
            List<Standard> standards,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var solutionStandardsModel = new SolutionStandardsModel(
                solution.CatalogueItem,
                standards,
                new List<string>(),
                contentStatus);

            mockService.GetSolutionWithBasicInformation(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetSolutionStandardsForMarketing(solution.CatalogueItemId).Returns(standards);

            mockService.GetWorkOffPlans(solution.CatalogueItemId).Returns(new List<WorkOffPlan>());

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.Standards(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(solutionStandardsModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationTypes_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.ApplicationTypes(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationTypes_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.ApplicationTypes(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ApplicationTypes_ValidSolutionForId_ReturnsExpectedViewResult(
            CatalogueItemId id,
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;

            var expectedModel = new ApplicationTypesModel(catalogueItem, contentStatus);
            solutionsService.GetSolutionWithBasicInformation(id).Returns(catalogueItem);

            solutionsService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            var actual = (await controller.ApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CheckEpics_ValidId_ResultAsExpected(
            CatalogueItem solution,
            [Frozen] ISolutionsService mockSolutionsService,
            SolutionsController controller)
        {
            var catalogueItemId = solution.Id;
            var capabilityItem = solution.CatalogueItemCapabilities.First();
            var capabilityId = capabilityItem.Capability.Id;
            var capability = solution.CatalogueItemCapabilities.First();

            var expectedModel = new SolutionCheckEpicsModel(capability, solution);

            mockSolutionsService.GetSolutionCapability(catalogueItemId, capabilityId).Returns(solution);

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(em => em.BackLinkText).Excluding(em => em.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CheckEpics_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            int capabilityId,
            [Frozen] ISolutionsService mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService.GetSolutionCapability(catalogueItemId, capabilityId).Returns(default(CatalogueItem));

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should()
                .Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_ValidIds_ResultAsExpected(
            AdditionalService additionalSolution,
            Solution solution,
            [Frozen] ISolutionsService mockSolutionsService,
            SolutionsController controller)
        {
            var catalogueItemIdAdditional = additionalSolution.CatalogueItemId;
            var capabilityItem = additionalSolution.CatalogueItem.CatalogueItemCapabilities.First();
            var capabilityId = capabilityItem.Capability.Id;
            var capability = additionalSolution.CatalogueItem.CatalogueItemCapabilities.First();

            var expectedModel = new SolutionCheckEpicsModel(capability, additionalSolution.CatalogueItem, additionalSolution.CatalogueItemId);

            mockSolutionsService.GetSolutionCapability(additionalSolution.CatalogueItemId, capabilityId).Returns(additionalSolution.CatalogueItem);

            mockSolutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

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
        [MockAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            AdditionalService additionalSolution,
            Solution solution,
            int capabilityId,
            [Frozen] ISolutionsService mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService.GetSolutionCapability(additionalSolution.CatalogueItemId, capabilityId).Returns(default(CatalogueItem));

            mockSolutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller
                    .CheckEpicsAdditionalServices(solution.CatalogueItemId, additionalSolution.CatalogueItemId, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {additionalSolution.CatalogueItemId} with Capability Id: {capabilityId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_NullSolution_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            int capabilityId,
            [Frozen] ISolutionsService mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller
                    .CheckEpicsAdditionalServices(catalogueItemId, additionalServiceId, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Description_ValidId_GetsSolutionFromService(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            mockService.GetSolutionWithBasicInformation(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            await controller.Description(solution.CatalogueItemId);

            await mockService.Received().GetSolutionWithBasicInformation(solution.CatalogueItemId);
            await mockService.Received().GetContentStatusForCatalogueItem(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Description_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.Description(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Description_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var solutionDescriptionModel = new SolutionDescriptionModel(catalogueItem, contentStatus);

            mockService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.Description(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(solutionDescriptionModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_ValidId_InvokesGetSolutionOverview(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            await controller.Features(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Features_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.Features(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(new SolutionFeaturesModel(catalogueItem, contentStatus));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.HostingType(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.HostingType(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] ISolutionsService solutionsServiceMock,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var expectedModel = new HostingTypesModel(catalogueItem, contentStatus);
            solutionsServiceMock.GetSolutionWithBasicInformation(solution.CatalogueItemId).Returns(catalogueItem);

            solutionsServiceMock.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.HostingType(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DataProcessingInformation_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithDataProcessingInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.DataProcessingInformation(id);

            await mockService.Received().GetSolutionWithDataProcessingInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DataProcessingInformation_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller)
        {
            mockService.GetSolutionWithDataProcessingInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.DataProcessingInformation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DataProcessingInformation_SuspendedSolution_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] ISolutionsService solutionsServiceMock,
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;

            solutionsServiceMock.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId).Returns(catalogueItem);

            var actual = (await controller.DataProcessingInformation(solution.CatalogueItemId)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(controller.Description));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DataProcessingInformation_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] ISolutionsService solutionsServiceMock,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var expectedModel = new DataProcessingInformationModel(catalogueItem, contentStatus);
            solutionsServiceMock.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId).Returns(catalogueItem);

            solutionsServiceMock.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.DataProcessingInformation(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(
            [Frozen] Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.Implementation(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.Implementation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_ReturnsExpectedViewResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            var implementationTimescalesModel = new ImplementationTimescalesModel(catalogueItem, contentStatus);

            mockService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.Implementation(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(implementationTimescalesModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPrice_ValidId_GetsSolutionFromService(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionWithCataloguePrice(Arg.Any<CatalogueItemId>()).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            await controller.ListPrice(catalogueItem.Id);

            await mockService.Received().GetSolutionWithCataloguePrice(Arg.Any<CatalogueItemId>());
            await mockService.Received().GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPrice_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithCataloguePrice(id).Returns(default(CatalogueItem));

            var actual = (await controller.ListPrice(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var mockSolutionListPriceModel = new ListPriceModel(catalogueItem, contentStatus);

            mockService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.ListPrice(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(mockSolutionListPriceModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServicePricePage_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IListPriceService mockListPriceService,
            SolutionsController controller,
            Solution solution,
            AdditionalService additionalService,
            CatalogueItemContentStatus contentStatus,
            CatalogueItem service)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            catalogueItem.AdditionalService = additionalService;
            service = catalogueItem.AdditionalService.CatalogueItem;
            var mockSolutionListPriceModel = new ListPriceModel(catalogueItem, service, contentStatus) { IndexValue = 4, };

            mockListPriceService.GetCatalogueItemWithListPrices(service.Id).Returns(service);

            mockSolutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            mockSolutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.AdditionalServicePrice(catalogueItem.Id, service.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("ListPrice");
            actual.Model.Should().BeEquivalentTo(mockSolutionListPriceModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.IndexValue).Excluding(m => m.Caption));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServicePricePage_SuspendedStatus_ReturnsRedirect(
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IListPriceService mockListPriceService,
            SolutionsController controller,
            Solution solution,
            AdditionalService additionalService,
            CatalogueItem service)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            catalogueItem.AdditionalService = additionalService;
            service = catalogueItem.AdditionalService.CatalogueItem;

            mockListPriceService.GetCatalogueItemWithListPrices(service.Id).Returns(service);

            mockSolutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.AdditionalServicePrice(catalogueItem.Id, service.Id);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServicePricePage_NullCatalogueItem_ReturnsError(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItem service,
            CatalogueItemId catalogueItemId)
        {
            solution.CatalogueItem = null;
            var catalogueItem = solution.CatalogueItem;

            mockService.GetSolutionWithCataloguePrice(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.AdditionalServicePrice(catalogueItemId, service.Id)).As<ViewResult>();

            actual.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServicePricePage_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IListPriceService mockListPriceService,
            SolutionsController controller,
            Solution solution,
            AssociatedService associatedService,
            CatalogueItemContentStatus contentStatus,
            CatalogueItem service)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            catalogueItem.AssociatedService = associatedService;
            service = catalogueItem.AssociatedService.CatalogueItem;
            var mockSolutionListPriceModel = new ListPriceModel(catalogueItem, service, contentStatus) { IndexValue = 5, };

            mockListPriceService.GetCatalogueItemWithListPrices(service.Id).Returns(service);

            mockSolutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            mockSolutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.AssociatedServicePrice(catalogueItem.Id, service.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("ListPrice");
            actual.Model.Should().BeEquivalentTo(mockSolutionListPriceModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.IndexValue).Excluding(m => m.Caption));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServicePricePage_SuspendedStatus_ReturnsRedirect(
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IListPriceService mockListPriceService,
            SolutionsController controller,
            Solution solution,
            AssociatedService associatedService)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            catalogueItem.AssociatedService = associatedService;
            var service = catalogueItem.AssociatedService.CatalogueItem;

            mockListPriceService.GetCatalogueItemWithListPrices(service.Id).Returns(service);

            mockSolutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.AssociatedServicePrice(catalogueItem.Id, service.Id);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServicePricePage_NullCatalogueItem_ReturnsError(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItem service,
            CatalogueItemId catalogueItemId)
        {
            solution.CatalogueItem = null;
            var catalogueItem = solution.CatalogueItem;

            mockService.GetSolutionWithCataloguePrice(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.AssociatedServicePrice(catalogueItemId, service.Id)).As<ViewResult>();

            actual.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Interoperability_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.Interoperability(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Interoperability_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.Interoperability(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Interoperability_ValidSolutionForId_ReturnsExpectedViewResult(
            CatalogueItemContentStatus contentStatus,
            List<SolutionIntegration> solutionIntegrations,
            List<IntegrationType> integrationTypes,
            Solution solution,
            [Frozen] ISolutionsService mockSolutionService,
            SolutionsController controller)
        {
            solutionIntegrations.Zip(integrationTypes).ToList().ForEach(x => x.First.IntegrationType = x.Second);
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.Integrations = solutionIntegrations;

            var expectedViewData = new InteroperabilityModel(catalogueItem, contentStatus);

            mockSolutionService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            mockSolutionService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var actual = (await controller.Interoperability(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SupplierDetails_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            mockService.GetSolutionWithSupplierDetails(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            await controller.SupplierDetails(id);

            await mockService.Received().GetSolutionWithSupplierDetails(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SupplierDetails_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithSupplierDetails(id).Returns(default(CatalogueItem));

            var actual = (await controller.SupplierDetails(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SupplierDetails_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var expectedSolutionSupplierDetailsModel = new SolutionSupplierDetailsModel(catalogueItem, contentStatus);

            mockService.GetSolutionWithSupplierDetails(id).Returns(catalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            var actual = (await controller.SupplierDetails(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedSolutionSupplierDetailsModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServices_ValidId_InvokesGetSolution(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var additionalServices = solution.AdditionalServices.Select(add => add.CatalogueItem).ToList();

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            mockService.GetPublishedAdditionalServicesForSolution(id).Returns(additionalServices);

            await controller.AdditionalServices(id);

            await mockService.Received().GetSolutionWithBasicInformation(id);
            await mockService.Received().GetContentStatusForCatalogueItem(id);
            await mockService.Received().GetPublishedAdditionalServicesForSolution(id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServices_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.AdditionalServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServices_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var id = solution.CatalogueItemId;

            var additionalServices = solution.AdditionalServices.Select(add => add.CatalogueItem).ToList();
            var expectedAdditionalServicesModel =
                new AdditionalServicesModel(solution.CatalogueItem, additionalServices, contentStatus);

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(id).Returns(contentStatus);

            mockService.GetPublishedAdditionalServicesForSolution(id).Returns(additionalServices);

            var actual = (await controller.AdditionalServices(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedAdditionalServicesModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalServices_SuspendedStatus_ReturnsRedirect(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution)
        {
            var id = solution.CatalogueItemId;
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Suspended;

            mockService.GetSolutionWithBasicInformation(id).Returns(solution.CatalogueItem);

            var result = await controller.AdditionalServices(id);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ServiceLevelAgreement_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithServiceLevelAgreements(id).Returns(default(CatalogueItem));

            var actual = (await controller.ServiceLevelAgreement(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ServiceLevelAgreement_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var expectedModel = new ServiceLevelAgreementDetailsModel(solution.CatalogueItem, contentStatus);

            mockService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            var actual = (await controller.ServiceLevelAgreement(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DevelopmentPlans_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithBasicInformation(id).Returns(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DevelopmentPlans_ValidSolutionForId_ReturnsExpectViewResult(
            [Frozen] ISolutionsService mockService,
            SolutionsController controller,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var expectedModel = new DevelopmentPlansModel(solution.CatalogueItem, new List<WorkOffPlan>(), contentStatus);

            mockService.GetSolutionWithBasicInformation(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetContentStatusForCatalogueItem(solution.CatalogueItemId).Returns(contentStatus);

            mockService.GetWorkOffPlans(solution.CatalogueItemId).Returns(new List<WorkOffPlan>());

            var actual = (await controller.DevelopmentPlans(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
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

        [Theory]
        [MockAutoData]
        public static void SolutionSort_ReturnsViewWithModel(
            SolutionsController controller)
        {
            var result = controller.SolutionSort().As<PartialViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<SolutionSortModel>();
        }
    }
}
