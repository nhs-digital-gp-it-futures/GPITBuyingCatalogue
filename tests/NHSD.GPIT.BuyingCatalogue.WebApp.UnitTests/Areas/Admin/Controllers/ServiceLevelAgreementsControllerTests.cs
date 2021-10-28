﻿using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class ServiceLevelAgreementsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceLevelAgreementsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Index_NullSla_Redirects(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solution.ServiceLevelAgreement = null;
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);
            slaService.Setup(s => s.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(default(ServiceLevelAgreements));

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.AddSlaLevel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Index_NotNullSla_Redirects(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);
            slaService.Setup(s => s.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Index_InvalidSolution_ReturnsBadReques(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddSlaLevel_ModelPopulatedCorrectly(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            solution.ServiceLevelAgreement = null;
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var expectedModel = new AddSlaTypeModel(solution.CatalogueItem);

            var actual = await controller.AddSlaLevel(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<AddSlaTypeModel>();
            var model = actual.As<ViewResult>().Model.As<AddSlaTypeModel>();
            model.Should()
                    .BeEquivalentTo(
                    expectedModel,
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddSlaLevel_NullSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AddSlaLevel(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaLevel_Valid(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };

            slaService.Setup(s => s.AddServiceLevelAsync(It.IsAny<AddSlaModel>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.AddSlaLevel(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.AddServiceLevelAsync(It.IsAny<AddSlaModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaLevel_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AddSlaLevel(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaLevel_ModelError(
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            controller.ModelState.AddModelError("Test", "A test error");

            var actual = await controller.AddSlaLevel(itemId, new AddSlaTypeModel());

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<AddSlaTypeModel>();
            var model = actual.As<ViewResult>().Model.As<AddSlaTypeModel>();
            model.Should()
                    .BeEquivalentTo(
                    new AddSlaTypeModel(),
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceLevelAgreement(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            var expectedModel = new EditServiceLevelAgreementModel(solution.CatalogueItem, solution.ServiceLevelAgreement);

            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);
            slaService.Setup(s => s.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

            var actual = await controller.EditServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceLevelAgreement_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSlaLevel_ModelPopulatedCorrectly(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);
            slaService.Setup(s => s.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

            var expectedModel = new AddSlaTypeModel(solution.CatalogueItem);

            var actual = await controller.EditSlaLevel(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<AddSlaTypeModel>();
            actual.As<ViewResult>().Model.As<AddSlaTypeModel>().SlaLevel.Should().Be(solution.ServiceLevelAgreement.SlaType);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSlaLevel_NullSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditSlaLevel(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaLevel_Valid(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };

            slaService.Setup(s => s.UpdateServiceLevelTypeAsync(solution.CatalogueItem, It.IsAny<SlaType>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditSlaLevel(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.UpdateServiceLevelTypeAsync(solution.CatalogueItem, It.IsAny<SlaType>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaLevel_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditSlaLevel(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaLevel_ModelError(
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            controller.ModelState.AddModelError("Test", "A test error");

            var actual = await controller.EditSlaLevel(itemId, new AddSlaTypeModel());

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<AddSlaTypeModel>();
            var model = actual.As<ViewResult>().Model.As<AddSlaTypeModel>();
            model.Should()
                    .BeEquivalentTo(
                    new AddSlaTypeModel(),
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddServiceAvailabilityTimes(solutionId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceAvailabilityTimes_Valid(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.AddServiceAvailabilityTimes(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be("AddEditServiceAvailabilityTimes");
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceAvailabilityTimes_InvalidModel(
            CatalogueItemId solutionId,
            EditServiceAvailabilityTimesModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.AddServiceAvailabilityTimes(solutionId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddServiceAvailabilityTimes(solutionId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceAvailabilityTimes_ValidModel(
            Solution solution,
            EditServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.AddServiceAvailabilityTimes(solution.CatalogueItemId, model);

            serviceLevelAgreementsService.Verify(s => s.SaveServiceAvailabilityTimes(solution.CatalogueItem, It.IsAny<ServiceAvailabilityTimesModel>()));

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId))
                .ReturnsAsync(default(ServiceAvailabilityTimes));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_Valid(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be("AddEditServiceAvailabilityTimes");
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_SingleServiceAvailabilityTimes(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(0);

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes)
            {
                CanDelete = false,
            };

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_ManyServiceAvailabilityTimes(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(1);

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes)
            {
                CanDelete = true,
            };

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_InvalidModel(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            EditServiceAvailabilityTimesModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId))
                .ReturnsAsync(default(ServiceAvailabilityTimes));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_ValidModel(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            EditServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id, model);

            serviceLevelAgreementsService.Verify(s => s.UpdateServiceAvailabilityTimes(solution.CatalogueItem, serviceAvailabilityTimes.Id, It.IsAny<ServiceAvailabilityTimesModel>()));

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId))
                .ReturnsAsync(default(ServiceAvailabilityTimes));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_Valid(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            var expectedModel = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId))
                .ReturnsAsync(default(ServiceAvailabilityTimes));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_ValidModel(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
                .ReturnsAsync(serviceAvailabilityTimes);

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id, model);

            serviceLevelAgreementsService.Verify(s => s.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id));

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }
    }
}
