using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AdditionalServiceListPriceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServiceListPriceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new ManageListPricesModel(solution.CatalogueItem, additionalService.CatalogueItem, additionalService.CatalogueItem.CataloguePrices);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.Index(solution.CatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("ListPrices/ManageListPrices");
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.AddListPriceUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_SolutionNotFound(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.Index(solutionId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.Index(solution.CatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new ListPriceTypeModel(additionalService.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.ListPriceType(solution.CatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.ListPriceType(solution.CatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_InvalidModel_ReturnsView(
            Solution solution,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.ListPriceType(solution.CatalogueItemId, additionalService.CatalogueItemId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Flat_RedirectsCorrectly(
            Solution solution,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Flat;

            var result = controller.ListPriceType(solution.CatalogueItemId, additionalService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddFlatListPrice));
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Tiered_RedirectsCorrectly(
            Solution solution,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Tiered;

            var result = controller.ListPriceType(solution.CatalogueItemId, additionalService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new AddTieredListPriceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_WithPriceId_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice cataloguePrice,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(cataloguePrice);

            var model = new AddTieredListPriceModel(solution.CatalogueItem, additionalService.CatalogueItem, cataloguePrice);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePrice.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.DeleteListPriceUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(solutionId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_InvalidModel_ReturnsView(
            Solution solution,
            AdditionalService additionalService,
            AddTieredListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_Redirects(
            Solution solution,
            AdditionalService additionalService,
            AddTieredListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new TieredPriceTiersModel(solution.CatalogueItem, additionalService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.AddTieredPriceTierUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_SolutionNotFound(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(solutionId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_InvalidModel_ReturnsView(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            model.Tiers = price.CataloguePriceTiers.ToList();

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
                { "additionalServiceId", additionalService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(additionalService.CatalogueItem, price)
            {
                IsEditing = false,
            };

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsEditing_CorrectBacklink(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, true)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsNotEditing_CorrectBacklink(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalServiceId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_InvalidModel_ReturnsView(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = false;

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
                { "cataloguePriceId", price.CataloguePriceId },
                { "additionalServiceId", additionalService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredListPrice(solutionId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new EditTieredListPriceModel(solution.CatalogueItem, additionalService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.AddPricingTierUrl)
                            .Excluding(m => m.DeleteListPriceUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_InvalidModel_ReturnsView(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            additionalService.CatalogueItem.CataloguePrices.Add(price);

            model.Tiers = price.CataloguePriceTiers.ToList();

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_SamePublicationStatus(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            additionalService.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            await listPriceService.DidNotReceive().SetPublicationStatus(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            additionalService.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(
                        match => match.Description == pricingUnit.Description
                            && match.Definition == pricingUnit.Definition
                            && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            await listPriceService.Received().SetPublicationStatus(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalServiceId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceTierNotFound(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsEditing_CorrectBacklink(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, true);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsNotEditing_CorrectBacklink(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, false);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(additionalService.CatalogueItem, price, tier);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.DeleteTieredPriceTierUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_InvalidModel_ReturnsView(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditTieredPriceTierModel model,
            AdditionalServiceListPriceController controller)
        {
            model.CatalogueItemId = solutionId;
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTieredPriceTier(model.CatalogueItemId, additionalServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsInfiniteRange(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = additionalServiceId;
            model.IsInfiniteRange = true;

            _ = await controller.EditTieredPriceTier(solutionId, additionalServiceId, model.CataloguePriceId, model.TierId!.Value, model);

            await listPriceService.Received().UpdateListPriceTier(
                    model.CatalogueItemId,
                    model.CataloguePriceId,
                    model.TierId!.Value,
                    model.Price!.Value,
                    model.LowerRange!.Value,
                    null);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsNotInfiniteRange(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = additionalServiceId;
            model.IsInfiniteRange = false;

            _ = await controller.EditTieredPriceTier(solutionId, additionalServiceId, model.CataloguePriceId, model.TierId!.Value, model);

            await listPriceService.Received().UpdateListPriceTier(
                    model.CatalogueItemId,
                    model.CataloguePriceId,
                    model.TierId!.Value,
                    model.Price!.Value,
                    model.LowerRange!.Value,
                    model.UpperRange);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsEditing_Redirects(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = additionalServiceId;
            model.IsEditing = true;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(solutionId, additionalServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPriceTier(
                    model.CatalogueItemId,
                    model.CataloguePriceId,
                    model.TierId!.Value,
                    model.Price!.Value,
                    model.LowerRange!.Value,
                    null);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsNotEditing_Redirects(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = additionalServiceId;
            model.IsEditing = false;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(solutionId, additionalServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPriceTier(
                    model.CatalogueItemId,
                    model.CataloguePriceId,
                    model.TierId!.Value,
                    model.Price!.Value,
                    model.LowerRange!.Value,
                    null);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, additionalServiceId, cataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_TierNotFound(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier cataloguePriceTier,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(cataloguePriceTier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new EditTierPriceModel(additionalService.CatalogueItem, price, cataloguePriceTier)
            {
                TierIndex = 0,
            };

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTier.Id, model.TierIndex)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_InvalidModel_ReturnsView(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTierPrice(solutionId, additionalServiceId, cataloguePriceId, tierId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_Redirects(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            [Frozen] IListPriceService service,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.EditTierPrice(solutionId, additionalServiceId, cataloguePriceId, tierId, model)).As<RedirectToActionResult>();

            await service.Received().UpdateTierPrice(additionalServiceId, cataloguePriceId, tierId, model.Price!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteListPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                additionalService.CatalogueItem.Name,
                "This list price will be deleted");

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            additionalService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeleteListPrice(additionalService.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_PublishedPrice_DoesNotDelete(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            additionalService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeleteListPrice(solution.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                additionalService.CatalogueItem.Name,
                "This pricing tier will be deleted");

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, tierId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_PublishedPrice_DoesNotDelete(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_IsEditing_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model, true)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_AdditionalServiceNotFound(
            Solution solution,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddFlatListPrice(solution.CatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new AddEditFlatListPriceModel(additionalService.CatalogueItem);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddFlatListPrice(solution.CatalogueItemId,  additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_InvalidModel(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditFlatListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddFlatListPrice(solutionId, additionalServiceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_Redirects(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.AddFlatListPrice(solutionId, additionalServiceId, model)).As<RedirectToActionResult>();

            var pricingUnit = model.GetPricingUnit();
            await listPriceService.Received().AddListPrice(
                    additionalServiceId,
                    Arg.Is<CataloguePrice>(
                        p => p.CataloguePriceType == CataloguePriceType.Flat
                            && p.ProvisioningType == model.SelectedProvisioningType!.Value
                            && p.TimeUnit == model.GetBillingPeriod()
                            && p.PricingUnit.Description == pricingUnit.Description
                            && p.PricingUnit.Definition == pricingUnit.Definition
                            && p.CataloguePriceCalculationType == CataloguePriceCalculationType.SingleFixed
                            && p.PublishedStatus == model.SelectedPublicationStatus!.Value
                            && p.CataloguePriceTiers.First().Price == model.Price!.Value));

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_AdditionalServiceNotFound(
            CatalogueItemId solutionId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solutionId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(solutionId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_PriceNotFound(
            Solution solution,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditFlatListPriceModel(additionalService.CatalogueItem, price);

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    model,
                    opt =>
                        opt.Excluding(m => m.BackLink)
                            .Excluding(m => m.DeleteListPriceUrl));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_InvalidModel(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            AddEditFlatListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditFlatListPrice(solutionId, additionalServiceId, cataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_SamePublicationStatus(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            additionalService.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType(),
                    model.Price!.Value);

            await listPriceService.DidNotReceive().SetPublicationStatus(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_Redirects(
            Solution solution,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            additionalService.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType(),
                    model.Price!.Value);

            await listPriceService.Received().SetPublicationStatus(
                    additionalService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }
    }
}
