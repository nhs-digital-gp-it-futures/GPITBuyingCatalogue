using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceLevelAgreementsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Index_NullSla_Redirects(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solution.ServiceLevelAgreement = null;
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);
            slaService.GetServiceLevelAgreementForSolution(itemId).Returns(default(ServiceLevelAgreements));

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.AddServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Index_NotNullSla_Redirects(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);
            slaService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Index_InvalidSolution_ReturnsBadReques(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.Index(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddServiceLevelAgreement_ModelPopulatedCorrectly(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            solution.ServiceLevelAgreement = null;
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

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
        [MockAutoData]
        public static async Task Get_AddServiceLevelAgreement_NullSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.AddServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddServiceLevelAgreement_Valid(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var actual = await controller.AddServiceLevelAgreement(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            await slaService.Received().AddServiceLevelAgreement(Arg.Any<AddSlaModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddServiceLevelAgreement_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.AddServiceLevelAgreement(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_EditServiceLevelAgreement(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            var expectedModel = new EditServiceLevelAgreementModel(solution.CatalogueItem, solution.ServiceLevelAgreement);

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);
            slaService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

            var actual = await controller.EditServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevelAgreement_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.EditServiceLevelAgreement(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSlaLevel_ModelPopulatedCorrectly(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);
            slaService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

            var expectedModel = new AddSlaTypeModel(solution.CatalogueItem);

            var actual = await controller.EditServiceLevelAgreementType(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeOfType<AddSlaTypeModel>();
            actual.As<ViewResult>().Model.As<AddSlaTypeModel>().SlaLevel.Should().Be(solution.ServiceLevelAgreement.SlaType);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSlaLevel_NullSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.EditServiceLevelAgreementType(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSlaLevel_Valid_NoChangeInSlaType_NoSave(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            solution.ServiceLevelAgreement.SlaType = slaType;
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditServiceLevelAgreementType(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            await slaService.DidNotReceive().UpdateServiceLevelTypeAsync(solution.CatalogueItem, Arg.Any<SlaType>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSlaLevel_Valid_ChangeType_RedirectsToConfirmation(
                  [Frozen] ISolutionsService solutionsService,
                  ServiceLevelAgreementsController controller,
                  CatalogueItemId itemId,
                  Solution solution)
        {
            solution.ServiceLevelAgreement.SlaType = SlaType.Type1;
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = SlaType.Type2,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditServiceLevelAgreementType(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreementTypeConfirmation));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSlaLevel_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution,
            SlaType slaType)
        {
            var addModel = new AddSlaTypeModel(solution.CatalogueItem)
            {
                SlaLevel = slaType,
            };
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.EditServiceLevelAgreementType(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_AddServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.AddServiceAvailabilityTimes(solutionId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddServiceAvailabilityTimes_Valid(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.AddServiceAvailabilityTimes(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be("AddEditServiceAvailabilityTimes");
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_AddServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.AddServiceAvailabilityTimes(solutionId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddServiceAvailabilityTimes_ValidModel(
            Solution solution,
            EditServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.AddServiceAvailabilityTimes(solution.CatalogueItemId, model);

            await serviceLevelAgreementsService.Received().SaveServiceAvailabilityTimes(solution.CatalogueItem, Arg.Any<ServiceAvailabilityTimesModel>());

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            Solution solution,
            int serviceAvailabilityTimesId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId).Returns(default(ServiceAvailabilityTimes));

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_Valid(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be("AddEditServiceAvailabilityTimes");
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_SingleServiceAvailabilityTimes(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(0);

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes)
            {
                CanDelete = false,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceAvailabilityTimes_ManyServiceAvailabilityTimes(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(1);

            var expectedModel = new EditServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes)
            {
                CanDelete = true,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            Solution solution,
            int serviceAvailabilityTimesId,
            EditServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId).Returns(default(ServiceAvailabilityTimes));

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceAvailabilityTimes_ValidModel(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            EditServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.EditServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id, model);

            await serviceLevelAgreementsService.Received().UpdateServiceAvailabilityTimes(solution.CatalogueItem, serviceAvailabilityTimes.Id, Arg.Any<ServiceAvailabilityTimesModel>());

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            Solution solution,
            int serviceAvailabilityTimesId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId).Returns(default(ServiceAvailabilityTimes));

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceAvailabilityTimes_Valid(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            var expectedModel = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_NullSolution(
            CatalogueItemId solutionId,
            int serviceAvailabilityTimesId,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_NullServiceAvailabilityTimes(
            Solution solution,
            int serviceAvailabilityTimesId,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId).Returns(default(ServiceAvailabilityTimes));

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimesId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteServiceAvailabilityTimes_ValidModel(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            DeleteServiceAvailabilityTimesModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(serviceAvailabilityTimes);

            var result = await controller.DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id, model);

            await serviceLevelAgreementsService.Received().DeleteServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.AddContact(itemId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddSlaContact_ValidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var expectedModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel(solution.CatalogueItem);

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
        [MockAutoData]
        public static async Task Post_AddSlaContact_Valid(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel
            {
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var actual = await controller.AddContact(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.AddContact(itemId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_EditSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.EditContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSlaContact_InvalidContactId(
            int contactId,
            Solution solution,
            CatalogueItemId itemId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.EditContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSlaContact_ValidSolution(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

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
        [MockAutoData]
        public static async Task Post_EditSlaContact_Valid(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel()
            {
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow,
            };

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var actual = await controller.EditContact(itemId, expectedContact.Id, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            await slaService.Received().EditSlaContact(Arg.Any<ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.EditContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSlaContact_InvalidContact(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.EditContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_DeleteSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var actual = await controller.DeleteContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteSlaContact_InvalidContactId(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.DeleteContact(itemId, contactId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteSlaContact_ValidSolution(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementService,
            ServiceLevelAgreementsController controller,
            Solution solution,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementService.GetServiceLevelAgreementForSolution(itemId).Returns(solution.ServiceLevelAgreement);

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
        [MockAutoData]
        public static async Task Post_DeleteSlaContact_Valid(
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService slaService,
            ServiceLevelAgreementsController controller,
            CatalogueItemId itemId,
            Solution solution)
        {
            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var expectedContact = solution.ServiceLevelAgreement.Contacts.FirstOrDefault();

            var actual = await controller.DeleteContact(itemId, expectedContact.Id, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            await slaService.Received().DeleteSlaContact(Arg.Any<int>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteSlaContact_InvalidSolution(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(default(CatalogueItem));

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            var actual = await controller.DeleteContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteSlaContact_InvalidContact(
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller,
            int contactId,
            CatalogueItemId itemId,
            Solution solution)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(itemId).Returns(solution.CatalogueItem);

            var addModel = new WebApp.Areas.Admin.Models.ServiceLevelAgreements.EditSLAContactModel();

            solution.ServiceLevelAgreement.Contacts.Clear();

            var actual = await controller.DeleteContact(itemId, contactId, addModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Contact found for Id: {contactId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.AddServiceLevel(solutionId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddServiceLevel_ValidSolution(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new AddEditServiceLevelModel(solution.CatalogueItem);

            var result = await controller.AddServiceLevel(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_AddServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            AddEditServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.AddServiceLevel(solutionId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddServiceLevel_ValidRequest(
            Solution solution,
            AddEditServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.AddServiceLevel(solution.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            await serviceLevelAgreementsService.Received().AddServiceLevel(
                    solution.CatalogueItemId,
                    Arg.Is<EditServiceLevelModel>(
                        m => string.Equals(model.ServiceType, m.ServiceType)
                        && string.Equals(model.ServiceLevel, m.ServiceLevel)
                        && string.Equals(model.HowMeasured, m.HowMeasured)
                        && model.CreditsApplied == m.CreditsApplied));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceLevel(solutionId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            var expectedModel = new AddEditServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevel.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_EditServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            AddEditServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceLevel(solutionId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            AddEditServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            AddEditServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevel(solution.CatalogueItemId, serviceLevel.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            await serviceLevelAgreementsService.Received().UpdateServiceLevel(
                    solution.CatalogueItemId,
                    serviceLevel.Id,
                    Arg.Is<EditServiceLevelModel>(
                        m => string.Equals(model.ServiceType, m.ServiceType)
                        && string.Equals(model.ServiceLevel, m.ServiceLevel)
                        && string.Equals(model.HowMeasured, m.HowMeasured)
                        && model.CreditsApplied == m.CreditsApplied));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.DeleteServiceLevel(solutionId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevelId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            var expectedModel = new DeleteServiceLevelModel(solution.CatalogueItem, serviceLevel);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_DeleteServiceLevel_NullSolution(
            CatalogueItemId solutionId,
            int serviceLevelId,
            DeleteServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.DeleteServiceLevel(solutionId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteServiceLevel_NullServiceLevel(
            Solution solution,
            int serviceLevelId,
            DeleteServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement.ServiceLevels = new HashSet<SlaServiceLevel>();

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevelId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Service Level found for Id: {serviceLevelId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteServiceLevel_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            SlaServiceLevel serviceLevel,
            DeleteServiceLevelModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.ServiceLevels.Add(serviceLevel);
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
            await serviceLevelAgreementsService.Received().DeleteServiceLevel(solution.CatalogueItemId, serviceLevel.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevelAgreementTypeConfirmation_NullSolution(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceLevelAgreementTypeConfirmation(solutionId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditServiceLevelAgreementTypeConfirmation_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            var expectedModel = new EditSlaTypeConfirmationModel(solution.CatalogueItem);

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevelAgreementTypeConfirmation(solution.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().BeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceLevelAgreementTypeConfirmation_NullSolution(
            CatalogueItemId solutionId,
            EditSlaTypeConfirmationModel model,
            [Frozen] ISolutionsService solutionsService,
            ServiceLevelAgreementsController controller)
        {
            solutionsService.GetSolutionWithServiceLevelAgreements(solutionId).Returns(default(CatalogueItem));

            var result = await controller.EditServiceLevelAgreementTypeConfirmation(solutionId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditServiceLevelAgreementTypeConfirmation_ValidRequest(
            Solution solution,
            ServiceLevelAgreements serviceLevelAgreement,
            EditSlaTypeConfirmationModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            ServiceLevelAgreementsController controller)
        {
            serviceLevelAgreement.SlaType = SlaType.Type1;
            solution.ServiceLevelAgreement = serviceLevelAgreement;

            solutionsService.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditServiceLevelAgreementTypeConfirmation(solution.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.EditServiceLevelAgreement));
            await serviceLevelAgreementsService.Received().UpdateServiceLevelTypeAsync(solution.CatalogueItem, SlaType.Type2);
        }
    }
}
