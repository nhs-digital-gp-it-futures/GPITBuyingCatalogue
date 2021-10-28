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
            slaService.Setup(s => s.GetAllServiceLevelAgreementsForSolution(itemId)).ReturnsAsync(default(ServiceLevelAgreements));

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
            slaService.Setup(s => s.GetAllServiceLevelAgreementsForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

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
            slaService.Setup(s => s.GetAllServiceLevelAgreementsForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

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
            slaService.Setup(s => s.GetAllServiceLevelAgreementsForSolution(itemId)).ReturnsAsync(solution.ServiceLevelAgreement);

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
    }
}
