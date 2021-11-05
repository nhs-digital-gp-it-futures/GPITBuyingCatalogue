using System;
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
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.AddServiceLevelAgreement));
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
        public static async Task Get_AddServiceLevelAgreement_ModelPopulatedCorrectly(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            solution.ServiceLevelAgreement = null;
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var expectedModel = new AddSlaTypeModel(solution.CatalogueItem);

            var actual = await controller.AddServiceLevelAgreement(itemId);

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
        public static async Task Get_AddServiceLevelAgreement_NullSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AddServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevelAgreement_Valid(
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

            slaService.Setup(s => s.AddServiceLevelAgreement(It.IsAny<AddSlaModel>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.AddServiceLevelAgreement(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.AddServiceLevelAgreement(It.IsAny<AddSlaModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevelAgreement_InvalidSolution(
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

            var actual = await controller.AddServiceLevelAgreement(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevelAgreement_ModelError(
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            controller.ModelState.AddModelError("Test", "A test error");

            var actual = await controller.AddServiceLevelAgreement(itemId, new AddSlaTypeModel());

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

            var actual = await controller.EditServiceLevelAgreementType(itemId);

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

            var actual = await controller.EditServiceLevelAgreementType(itemId);

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

            var actual = await controller.EditServiceLevelAgreementType(itemId, addModel);

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

            var actual = await controller.EditServiceLevelAgreementType(itemId, addModel);

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

            var actual = await controller.EditServiceLevelAgreementType(itemId, new AddSlaTypeModel());

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

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
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

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id))
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
        public static async Task Post_DeleteServiceAvailabilityTimes_InvalidModel(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            DeleteServiceAvailabilityTimesModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AddContact(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddSlaContact_ValidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var expectedModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.AddContact(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            var model = actual.As<ViewResult>().Model.As<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            model.Should()
                    .BeEquivalentTo(
                    expectedModel,
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaContact_Valid(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel
            {
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow,
            };

            slaService.Setup(s => s.AddSLAContact(It.IsAny<CatalogueItem>(), It.IsAny<ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.AddContact(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.AddSLAContact(It.IsAny<CatalogueItem>(), It.IsAny<ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.AddContact(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSlaContact_ModelError(
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            controller.ModelState.AddModelError("Test", "A test error");

            var actual = await controller.AddContact(itemId, new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel());

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            var model = actual.As<ViewResult>().Model.As<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            model.Should()
                    .BeEquivalentTo(
                    new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel(),
                    opt => opt
                        .Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSlaContact_InvalidContactId(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.EditContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSlaContact_ValidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementService.Setup(slas => slas.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var expectedModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel(solution.CatalogueItem, expectedContact, solution.ServiceLevelAgreement);

            var actual = await controller.EditContact(itemId, expectedContact.Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            var model = actual.As<ViewResult>().Model.As<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            model.Should()
                    .BeEquivalentTo(
                    expectedModel,
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaContact_Valid(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel()
            {
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow,
            };

            slaService.Setup(s => s.EditSlaContact(It.IsAny<ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var actual = await controller.EditContact(itemId, expectedContact.Id, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.EditSlaContact(It.IsAny<ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.EditContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaContact_InvalidContact(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.EditContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSlaContact_ModelError(
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            controller.ModelState.AddModelError("Test", "A test error");

            var actual = await controller.EditContact(itemId, contactId, new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel());

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            var model = actual.As<ViewResult>().Model.As<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            model.Should()
                    .BeEquivalentTo(
                    new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel(),
                    opt => opt
                        .Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var actual = await controller.DeleteContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteSlaContact_InvalidContactId(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.DeleteContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteSlaContact_ValidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementService.Setup(slas => slas.GetServiceLevelAgreementForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var expectedModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel(solution.CatalogueItem, expectedContact);

            var actual = await controller.DeleteContact(itemId, expectedContact.Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            var model = actual.As<ViewResult>().Model.As<WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel>();
            model.Should()
                    .BeEquivalentTo(
                    expectedModel,
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteSlaContact_Valid(
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            slaService.Setup(s => s.DeleteSlaContact(It.IsAny<int>()));
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var actual = await controller.DeleteContact(itemId, expectedContact.Id, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            slaService.Verify(s => s.DeleteSlaContact(It.IsAny<int>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteSlaContact_InvalidSolution(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.DeleteContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteSlaContact_InvalidContact(
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.Setup(s => s.GetSolution(itemId)).ReturnsAsync(solution.CatalogueItem);

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.DeleteContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddServiceLevel(solutionId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceLevel_ValidSolution(
            Solution solution,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var expectedModel = new AddEditServiceLevelModel(solution.CatalogueItem);

            var result = await controller.AddServiceLevel(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevel_InvalidModelState(
            CatalogueItemId solutionId,
            AddEditServiceLevelModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.AddServiceLevel(solutionId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            AddEditServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddServiceLevel(solutionId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceLevel_ValidRequest(
            Solution solution,
            AddEditServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.AddServiceLevel(solution.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            serviceLevelAgreementsService.Verify(
                s => s.AddServiceLevel(
                    solution.CatalogueItemId,
                    It.Is<EditServiceLevelModel>(
                        m => string.Equals(model.ServiceType, m.ServiceType)
                        && string.Equals(model.ServiceLevel, m.ServiceLevel)
                        && string.Equals(model.HowMeasured, m.HowMeasured)
                        && Convert.ToBoolean((int)model.CreditsApplied) == m.CreditsApplied)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditServiceLevel(solutionId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            var expectedModel = new AddEditServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevel.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceLevel_InvalidModel(
            CatalogueItemId solutionId,
            int serviceLevelId,
            AddEditServiceLevelModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.EditServiceLevel(solutionId, serviceLevelId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            AddEditServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditServiceLevel(solutionId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            AddEditServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            AddEditServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevel.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            serviceLevelAgreementsService.Verify(
                s => s.UpdateServiceLevel(
                    solution.CatalogueItemId,
                    serviceLevel.Id,
                    It.Is<EditServiceLevelModel>(
                        m => string.Equals(model.ServiceType, m.ServiceType)
                        && string.Equals(model.ServiceLevel, m.ServiceLevel)
                        && string.Equals(model.HowMeasured, m.HowMeasured)
                        && Convert.ToBoolean((int)model.CreditsApplied) == m.CreditsApplied)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.DeleteServiceLevel(solutionId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            var expectedModel = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceLevel_InvalidModel(
            CatalogueItemId solutionId,
            int serviceLevelId,
            DeleteServiceLevelModel model,
            ServiceLevelAgreementsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.DeleteServiceLevel(solutionId, serviceLevelId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            DeleteServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.DeleteServiceLevel(solutionId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            DeleteServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            DeleteServiceLevelModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            serviceLevelAgreementsService.Verify(
                s => s.DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id));
        }
    }
}
