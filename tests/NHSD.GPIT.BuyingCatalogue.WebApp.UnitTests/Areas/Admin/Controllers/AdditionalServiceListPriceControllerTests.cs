using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
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
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new ManageListPricesModel(parentCatalogueItemId, additionalService.CatalogueItem, additionalService.CatalogueItem.CataloguePrices);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.Index(parentCatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

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
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.Index(solutionId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.Index(parentCatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new ListPriceTypeModel(additionalService.CatalogueItem);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.ListPriceType(parentCatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.ListPriceType(parentCatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_InvalidModel_ReturnsView(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.ListPriceType(parentCatalogueItemId, additionalService.CatalogueItemId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Flat_RedirectsCorrectly(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Flat;

            var result = controller.ListPriceType(parentCatalogueItemId, additionalService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddFlatListPrice));
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Tiered_RedirectsCorrectly(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            ListPriceTypeModel model,
            AdditionalServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Tiered;

            var result = controller.ListPriceType(parentCatalogueItemId, additionalService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new AddTieredListPriceModel(parentCatalogueItemId, additionalService.CatalogueItem);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_WithPriceId_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice cataloguePrice,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(cataloguePrice);

            var model = new AddTieredListPriceModel(parentCatalogueItemId, additionalService.CatalogueItem, cataloguePrice);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePrice.CataloguePriceId)).As<ViewResult>();

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
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(solutionId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(parentCatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_InvalidModel_ReturnsView(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            AddTieredListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_Redirects(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            AddTieredListPriceModel model,
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new TieredPriceTiersModel(parentCatalogueItemId, additionalService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
        public static async Task Get_TieredPriceTiers_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(parentCatalogueItemId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_PriceNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_InvalidModel_ReturnsView(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            model.Tiers = price.CataloguePriceTiers.ToList();

            var result = (await controller.TieredPriceTiers(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_Redirects(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            AdditionalServiceListPriceController controller)
        {
            var result = (await controller.TieredPriceTiers(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", parentCatalogueItemId },
                { "additionalServiceId", additionalService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsEditing_CorrectBacklink(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, true)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsNotEditing_CorrectBacklink(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalServiceId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_PriceNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_InvalidModel_ReturnsView(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_Redirects(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AdditionalServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = false;

            var result = (await controller.AddTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", parentCatalogueItemId },
                { "cataloguePriceId", price.CataloguePriceId },
                { "additionalServiceId", additionalService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_PriceNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new EditTieredListPriceModel(parentCatalogueItemId, additionalService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            additionalService.CatalogueItem.CataloguePrices.Add(price);

            model.Tiers = price.CataloguePriceTiers.ToList();

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_SamePublicationStatus(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

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
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

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
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredPriceTier(parentCatalogueItemId, additionalServiceId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceTierNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsEditing_CorrectBacklink(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, true);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsNotEditing_CorrectBacklink(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, false);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(additionalService.CatalogueItem, price, tier);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id)).As<ViewResult>();

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
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTierPrice(parentCatalogueItemId, additionalServiceId, cataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_PriceNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_TierNotFound(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditTierPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTier.Id, model.TierIndex)).As<ViewResult>();

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
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                additionalService.CatalogueItem.Name,
                "This list price will be deleted");

            var result = (await controller.DeleteListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_Redirects(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            additionalService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeleteListPrice(additionalService.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_PublishedPrice_DoesNotDelete(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            additionalService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeleteListPrice(parentCatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteTieredPriceTier_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                additionalService.CatalogueItem.Name,
                "This pricing tier will be deleted");

            var result = (await controller.DeleteTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId, tierId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_Redirects(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_PublishedPrice_DoesNotDelete(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_IsEditing_Redirects(
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id, model, true)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(additionalService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_AdditionalServiceNotFound(
            CatalogueItemId parentCatalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddFlatListPrice(parentCatalogueItemId, additionalServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            var model = new AddEditFlatListPriceModel(additionalService.CatalogueItem);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.AddFlatListPrice(parentCatalogueItemId, additionalService.CatalogueItemId)).As<ViewResult>();

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
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            int cataloguePriceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_ReturnsViewWithModel(
            CatalogueItemId parentCatalogueItemId,
            AdditionalService additionalService,
            CataloguePrice price,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServiceListPriceController controller)
        {
            additionalService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditFlatListPriceModel(additionalService.CatalogueItem, price);

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

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
            CatalogueItemId parentCatalogueItemId,
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

            additionalServicesService.GetAdditionalService(parentCatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(parentCatalogueItemId, additionalService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

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
