using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionListPriceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionListPriceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithModel(
            Solution solution,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new ManageListPricesModel(solution.CatalogueItem, solution.CatalogueItem.CataloguePrices);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.Index(solution.CatalogueItemId)).As<ViewResult>();

            solutionListPriceService.VerifyAll();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("ListPrices/ManageListPrices");
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SolutionNotFound_ReturnsNotFoundResult(
            CatalogueItemId solutionId,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solutionId))
                .ReturnsAsync((CatalogueItem)null);

            var result = (await controller.Index(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPriceType_ReturnsViewWithModel(
            Solution solution,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new ListPriceTypeModel(solution.CatalogueItem);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.ListPriceType(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPriceType_SolutionNotFound_ReturnsNotFoundResult(
            CatalogueItemId solutionId,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solutionId))
                .ReturnsAsync((CatalogueItem)null);

            var result = (await controller.ListPriceType(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ListPriceType_InvalidModel_ReturnsView(
            Solution solution,
            ListPriceTypeModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.ListPriceType(solution.CatalogueItemId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ListPriceType_Flat_RedirectsCorrectly(
            Solution solution,
            ListPriceTypeModel model,
            CatalogueSolutionListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Flat;

            var result = controller.ListPriceType(solution.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.ListPriceType));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ListPriceType_Tiered_RedirectsCorrectly(
            Solution solution,
            ListPriceTypeModel model,
            CatalogueSolutionListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Tiered;

            var result = controller.ListPriceType(solution.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddTieredListPrice));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredListPrice_ReturnsViewWithModel(
            Solution solution,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new AddTieredListPriceModel(solution.CatalogueItem);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, (int?)null)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredListPrice_WithPriceId_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice cataloguePrice,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(cataloguePrice);

            var model = new AddTieredListPriceModel(solution.CatalogueItem, cataloguePrice);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, cataloguePrice.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredListPrice_SolutionNotFound_ReturnsNotFoundResult(
            CatalogueItemId solutionId,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solutionId))
                .ReturnsAsync((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(solutionId, (int?)null)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddTieredListPrice_InvalidModel_ReturnsView(
            Solution solution,
            AddTieredListPriceModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddTieredListPrice_Redirects(
            Solution solution,
            AddTieredListPriceModel model,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            solutionListPriceService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TieredPriceTiers_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new TieredPriceTiersModel(solution.CatalogueItem, price);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TieredPriceTiers_SolutionNotFound_ReturnsNotFound(
            CatalogueItemId solutionId,
            CataloguePrice price,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solutionId))
                .ReturnsAsync((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(solutionId, price.CataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TieredPriceTiers_PriceNotFound_ReturnsNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_TieredPriceTiers_InvalidModel_ReturnsView(
            Solution solution,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            solution.CatalogueItem.CataloguePrices.Add(price);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            model.Tiers = price.CataloguePriceTiers.ToList();

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_TieredPriceTiers_Redirects(
            Solution solution,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            solutionListPriceService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddTieredPriceTierModel(solution.CatalogueItem, price);

            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredPriceTier_SolutionNotFound_ReturnsNotFound(
            CatalogueItemId solutionId,
            CataloguePrice price,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solutionId))
                .ReturnsAsync((CatalogueItem)null);

            var result = (await controller.AddTieredPriceTier(solutionId, price.CataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddTieredPriceTier_PriceNotFound_ReturnsNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solutionListPriceService.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddTieredPriceTier_InvalidModel_ReturnsView(
            Solution solution,
            CataloguePrice price,
            AddTieredPriceTierModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddTieredPriceTier_Redirects(
            Solution solution,
            CataloguePrice price,
            AddTieredPriceTierModel model,
            [Frozen] Mock<ISolutionListPriceService> solutionListPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            solutionListPriceService.VerifyAll();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
                { "cataloguePriceId", price.CataloguePriceId },
            });
        }
    }
}
