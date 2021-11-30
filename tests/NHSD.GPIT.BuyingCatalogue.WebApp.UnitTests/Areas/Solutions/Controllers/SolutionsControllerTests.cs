﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
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
        public static async Task Get_Index_GetsSolutionsFromService(
            [Frozen]Mock<ISolutionsFilterService> mockService,
            SolutionsController controller)
        {
            var pagedList = new PagedList<CatalogueItem>(new List<CatalogueItem>(), new PageOptions(string.Empty, string.Empty));
            var categoryModel = new CategoryFilterModel
            {
                CategoryFilters = new List<CapabilityCategoryFilter>(),
                FoundationCapabilities = new List<CapabilitiesFilter>(),
            };

            mockService.Setup(s => s.GetAllSolutionsFiltered(It.IsAny<PageOptions>(), null, null))
                .ReturnsAsync(pagedList);

            mockService.Setup(s => s.GetAllFrameworksAndCountForFilter())
                .ReturnsAsync(new List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>>());

            mockService.Setup(s => s.GetAllCategoriesAndCountForFilter(It.IsAny<string>()))
                .ReturnsAsync(categoryModel);

            await controller.Index(null, null, null, null);

            mockService.Verify(s => s.GetAllSolutionsFiltered(It.IsAny<PageOptions>(), null, null));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsExpectedViewResults(
            [Frozen] Mock<ISolutionsFilterService> mockService,
            SolutionsController controller)
        {
            var pagedList = new PagedList<CatalogueItem>(new List<CatalogueItem>(), new PageOptions(string.Empty, string.Empty));
            var categoryModel = new CategoryFilterModel
            {
                CategoryFilters = new List<CapabilityCategoryFilter>(),
                FoundationCapabilities = new List<CapabilitiesFilter>(),
            };

            mockService.Setup(s => s.GetAllSolutionsFiltered(It.IsAny<PageOptions>(), null, null))
                .ReturnsAsync(pagedList);

            mockService.Setup(s => s.GetAllFrameworksAndCountForFilter())
                .ReturnsAsync(new List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>>());

            mockService.Setup(s => s.GetAllCategoriesAndCountForFilter(It.IsAny<string>()))
                .ReturnsAsync(categoryModel);

            var actual = (await controller.Index(null, null, null, null)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeOfType(typeof(SolutionsModel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_QueryParameters_CorrectResults(
            [Frozen] Mock<ISolutionsFilterService> mockService,
            SolutionsController controller)
        {
            var pagedList = new PagedList<CatalogueItem>(new List<CatalogueItem>(), new PageOptions(string.Empty, string.Empty));
            var categoryModel = new CategoryFilterModel
            {
                CategoryFilters = new List<CapabilityCategoryFilter>(),
                FoundationCapabilities = new List<CapabilitiesFilter>(),
            };

            mockService.Setup(s => s.GetAllSolutionsFiltered(It.IsAny<PageOptions>(), null, null))
                .ReturnsAsync(pagedList);

            mockService.Setup(s => s.GetAllFrameworksAndCountForFilter())
                .ReturnsAsync(
                new List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>>
                {
                    new KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>(
                        new EntityFramework.Catalogue.Models.Framework { Id = "All", ShortName = "All" },
                        10),
                });

            mockService.Setup(s => s.GetAllCategoriesAndCountForFilter(It.IsAny<string>()))
                .ReturnsAsync(categoryModel);

            var actual = (await controller.Index(null, null, null, null)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeOfType(typeof(SolutionsModel));
            actual.Model.As<SolutionsModel>().SelectedFramework.Should().Be("All");
            actual.Model.As<SolutionsModel>().FrameworkFilters.Count.Should().Be(1);
            actual.Model.As<SolutionsModel>().FrameworkFilters[0].FrameworkId.Should().Be("All");
            actual.Model.As<SolutionsModel>().FrameworkFilters[0].Count.Should().Be(10);
            actual.Model.As<SolutionsModel>().FrameworkFilters[0].FrameworkFullName.Should().Be("All frameworks");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetPublishedAssociatedServicesForSolution(id))
                .ReturnsAsync(associatedServices);

            await controller.AssociatedServices(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
            mockService.Verify(s => s.GetPublishedAssociatedServicesForSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.ClientApplication = JsonSerializer.Serialize(clientApplication);
            var associatedServices = solution.CatalogueItem.SupplierServiceAssociations.Select(ssa => ssa.CatalogueItem).ToList();
            var associatedServicesModel = new AssociatedServicesModel(catalogueItem, associatedServices);

            solutionsServiceMock.Setup(s => s.GetSolutionOverview(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

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
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.Capabilities(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            var capabilitiesViewModel = new CapabilitiesViewModel(catalogueItem);
            mockService.Setup(s => s.GetSolutionOverview(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Capabilities(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(capabilitiesViewModel, opt => opt.Excluding(cvm => cvm.BackLink).Excluding(cvm => cvm.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Standards_GetSolutionAndStandardsFromService(
            Solution solution,
            List<Standard> standards,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            mockService.Setup(s => s.GetSolutionOverview(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            await controller.Standards(solution.CatalogueItemId);

            mockService.Verify(s => s.GetSolutionOverview(solution.CatalogueItemId));
            mockService.Verify(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Standards_SolutionDoesNotExist_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            SolutionsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var solutionStandardsModel = new SolutionStandardsModel(solution.CatalogueItem, standards, new List<string>());

            mockService.Setup(s => s.GetSolutionOverview(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForMarketing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            mockService.Setup(s => s.GetWorkOffPlans(solution.CatalogueItemId)).ReturnsAsync(new List<WorkOffPlan>());

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
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.ClientApplication = JsonSerializer.Serialize(clientApplication);

            var expectedModel = new ClientApplicationTypesModel(catalogueItem);
            solutionsService.Setup(s => s.GetSolutionOverview(id)).ReturnsAsync(catalogueItem);

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
            var capability = solution.CatalogueItemCapability(capabilityId);

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
            CatalogueItemId catalogueItemId,
            CatalogueItem additionalSolution,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            var catalogueItemIdAdditional = additionalSolution.Id;
            var capabilityItem = additionalSolution.CatalogueItemCapabilities.First();
            var capabilityId = capabilityItem.Capability.Id;
            var capability = additionalSolution.CatalogueItemCapability(capabilityId);

            var expectedModel = new SolutionCheckEpicsModel(capability, additionalSolution, catalogueItemIdAdditional);

            mockSolutionsService
                .Setup(s => s.GetAdditionalServiceCapability(catalogueItemIdAdditional, capabilityId))
                .ReturnsAsync(additionalSolution);

            var actual = (await controller.CheckEpicsAdditionalServices(
                    catalogueItemId,
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
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            int capabilityId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionsController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetAdditionalServiceCapability(catalogueItemIdAdditional, capabilityId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller
                    .CheckEpicsAdditionalServices(catalogueItemId, catalogueItemIdAdditional, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
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
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id)).ReturnsAsync((CatalogueItem)null);

            await controller.Description(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.Description(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var solutionDescriptionModel = new SolutionDescriptionModel(catalogueItem);

            mockService.Setup(s => s.GetSolutionOverview(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

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
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.Features(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionOverview(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Features(catalogueItem.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(new SolutionFeaturesModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.HostingType(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_NullSolutionForId_ReturnsBadRequestResult(
            CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HostingType(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidSolutionForId_ReturnsExpectedViewResult(
            CatalogueItemId id,
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            var expectedModel = new HostingTypesModel(catalogueItem);
            solutionsServiceMock.Setup(s => s.GetSolutionOverview(id)).ReturnsAsync(catalogueItem);

            var actual = (await controller.HostingType(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(
            [Frozen] CatalogueItemId id,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            mockService.Setup(s => s.GetSolutionOverview(id)).ReturnsAsync((CatalogueItem)null);

            await controller.Implementation(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            SolutionsController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            var implementationTimescalesModel = new ImplementationTimescalesModel(catalogueItem);

            mockService.Setup(s => s.GetSolutionOverview(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

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
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.Setup(s => s.GetSolutionOverview(It.IsAny<CatalogueItemId>())).ReturnsAsync(catalogueItem);

            await controller.ListPrice(catalogueItem.Id);

            mockService.Verify(s => s.GetSolutionOverview(catalogueItem.Id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            CatalogueItemId id)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var mockSolutionListPriceModel = new ListPriceModel(catalogueItem);

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ListPrice(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(mockSolutionListPriceModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.Interoperability(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            CatalogueItemId id,
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.Integrations = GetIntegrationsJson();

            var expectedViewData = new InteroperabilityModel(catalogueItem);

            mockSolutionService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Interoperability(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidId_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.SupplierDetails(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            var expectedSolutionSupplierDetailsModel = new SolutionSupplierDetailsModel(catalogueItem);

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(catalogueItem);

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
            SolutionsController controller)
        {
            var id = solution.CatalogueItemId;

            var additionalServices = solution.AdditionalServices.Select(add => add.CatalogueItem).ToList();

            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetPublishedAdditionalServicesForSolution(id))
                .ReturnsAsync(additionalServices);

            await controller.AdditionalServices(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
            mockService.Verify(s => s.GetPublishedAdditionalServicesForSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ServiceLevelAgreement_NullSolutionForId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            SolutionsController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            Solution solution)
        {
            var expectedModel = new ServiceLevelAgreementDetailsModel(
                solution.CatalogueItem,
                solution.ServiceLevelAgreement.ServiceHours,
                solution.ServiceLevelAgreement.Contacts,
                solution.ServiceLevelAgreement.ServiceLevels);

            mockService.Setup(s => s.GetSolutionOverview(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

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
            mockService.Setup(s => s.GetSolutionOverview(id))
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
            Solution solution)
        {
            var expectedModel = new DevelopmentPlansModel(solution.CatalogueItem, new List<WorkOffPlan>());

            mockService.Setup(s => s.GetSolutionOverview(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(new List<WorkOffPlan>());

            var actual = (await controller.DevelopmentPlans(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();

            actual.Model.Should().BeEquivalentTo(expectedModel);
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
