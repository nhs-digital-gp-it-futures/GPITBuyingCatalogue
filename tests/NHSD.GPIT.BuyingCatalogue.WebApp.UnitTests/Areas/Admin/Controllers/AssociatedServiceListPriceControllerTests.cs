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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AssociatedServiceListPriceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServiceListPriceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            var model = new ManageListPricesModel(supplierId, associatedService.CatalogueItem, associatedService.CatalogueItem.CataloguePrices);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.Index(supplierId, associatedService.CatalogueItemId)).As<ViewResult>();

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
        public static async Task Get_Index_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.Index(supplierId, associatedServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            var model = new ListPriceTypeModel(associatedService.CatalogueItem);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.ListPriceType(supplierId, associatedService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.ListPriceType(supplierId, associatedServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_InvalidModel_ReturnsView(
            int supplierId,
            AssociatedService associatedService,
            ListPriceTypeModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.ListPriceType(supplierId, associatedService.CatalogueItemId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Flat_RedirectsCorrectly(
            int supplierId,
            AssociatedService associatedService,
            ListPriceTypeModel model,
            AssociatedServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Flat;

            var result = controller.ListPriceType(supplierId, associatedService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddFlatListPrice));
        }

        [Theory]
        [MockAutoData]
        public static void Post_ListPriceType_Tiered_RedirectsCorrectly(
            int supplierId,
            AssociatedService associatedService,
            ListPriceTypeModel model,
            AssociatedServiceListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Tiered;

            var result = controller.ListPriceType(supplierId, associatedService.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            var model = new AddTieredListPriceModel(supplierId, associatedService.CatalogueItem);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.AddTieredListPrice(supplierId, associatedService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(supplierId, associatedServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_InvalidModel_ReturnsView(
            int supplierId,
            AssociatedService associatedService,
            AddTieredListPriceModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredListPrice(supplierId, associatedService.CatalogueItemId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_Redirects(
            int supplierId,
            AssociatedService associatedService,
            AddTieredListPriceModel model,
            AssociatedServiceListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(supplierId, associatedService.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            var model = new TieredPriceTiersModel(supplierId, associatedService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
        public static async Task Get_TieredPriceTiers_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(supplierId, associatedServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.TieredPriceTiers(supplierId, associatedService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_InvalidModel_ReturnsView(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            model.Tiers = price.CataloguePriceTiers.ToList();

            var result = (await controller.TieredPriceTiers(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            TieredPriceTiersModel model,
            AssociatedServiceListPriceController controller)
        {
            var result = (await controller.TieredPriceTiers(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "supplierId", supplierId },
                { "associatedServiceId", associatedService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(associatedService.CatalogueItem, price)
            {
                IsEditing = false,
            };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsEditing_CorrectBacklink(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, true)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsNotEditing_CorrectBacklink(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredPriceTier(supplierId, associatedServiceId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_InvalidModel_ReturnsView(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = false;

            var result = (await controller.AddTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "supplierId", supplierId },
                { "cataloguePriceId", price.CataloguePriceId },
                { "associatedServiceId", associatedService.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredListPrice(supplierId, associatedServiceId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(supplierId, associatedService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var model = new EditTieredListPriceModel(supplierId, associatedService.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            var result = (await controller.EditTieredListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            associatedService.CatalogueItem.CataloguePrices.Add(price);

            model.Tiers = price.CataloguePriceTiers.ToList();

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTieredListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_SamePublicationStatus(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            associatedService.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    associatedService.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            await listPriceService.DidNotReceive().SetPublicationStatus(
                    associatedService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    associatedService.CatalogueItemId,
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
                    associatedService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredPriceTier(supplierId, associatedServiceId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(supplierId, associatedService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceTierNotFound(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsEditing_CorrectBacklink(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id, true);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsNotEditing_CorrectBacklink(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            _ = await controller.EditTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id, false);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(associatedService.CatalogueItem, price, tier);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id)).As<ViewResult>();

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
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditTieredPriceTierModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTieredPriceTier(supplierId, associatedServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsInfiniteRange(
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = associatedServiceId;
            model.IsInfiniteRange = true;

            _ = await controller.EditTieredPriceTier(supplierId, associatedServiceId, model.CataloguePriceId, model.TierId!.Value, model);

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
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = associatedServiceId;
            model.IsInfiniteRange = false;

            _ = await controller.EditTieredPriceTier(supplierId, associatedServiceId, model.CataloguePriceId, model.TierId!.Value, model);

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
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = associatedServiceId;
            model.IsEditing = true;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(supplierId, associatedServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

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
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.CatalogueItemId = associatedServiceId;
            model.IsEditing = false;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(supplierId, associatedServiceId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

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
        public static async Task Get_EditTierPrice_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.EditTierPrice(supplierId, associatedServiceId, cataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTierPrice(supplierId, associatedService.CatalogueItemId, cataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_TierNotFound(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTierPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier cataloguePriceTier,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.CataloguePriceTiers.Add(cataloguePriceTier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            var model = new EditTierPriceModel(associatedService.CatalogueItem, price, cataloguePriceTier)
            {
                TierIndex = 0,
            };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditTierPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, cataloguePriceTier.Id, model.TierIndex)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_InvalidModel_ReturnsView(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTierPrice(supplierId, associatedServiceId, cataloguePriceId, tierId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_Redirects(
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            [Frozen] IListPriceService service,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.EditTierPrice(supplierId, associatedServiceId, cataloguePriceId, tierId, model)).As<RedirectToActionResult>();

            await service.Received().UpdateTierPrice(associatedServiceId, cataloguePriceId, tierId, model.Price!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteListPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                associatedService.CatalogueItem.Name,
                "This list price will be deleted");

            var result = (await controller.DeleteListPrice(supplierId, associatedService.CatalogueItemId, cataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            associatedService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.DeleteListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeleteListPrice(associatedService.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_PublishedPrice_DoesNotDelete(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            associatedService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.DeleteListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeleteListPrice(associatedService.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteTieredPriceTier_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            int tierId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                associatedService.CatalogueItem.Name,
                "This pricing tier will be deleted");

            var result = (await controller.DeleteTieredPriceTier(supplierId, associatedService.CatalogueItemId, cataloguePriceId, tierId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_PublishedPrice_DoesNotDelete(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeletePriceTier(associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_IsEditing_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id, model, true)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(associatedService.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_AssociatedServiceNotFound(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns((CatalogueItem)null);

            var result = (await controller.AddFlatListPrice(supplierId, associatedServiceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            var model = new AddEditFlatListPriceModel(associatedService.CatalogueItem) { PracticeReorganisation = PracticeReorganisationTypeEnum.None };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.AddFlatListPrice(supplierId, associatedService.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_InvalidModel(
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditFlatListPriceModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddFlatListPrice(supplierId, associatedServiceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_Redirects(
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            AssociatedServiceListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.AddFlatListPrice(supplierId, associatedServiceId, model)).As<RedirectToActionResult>();

            var pricingUnit = model.GetPricingUnit();
            await listPriceService.Received().AddListPrice(
                    associatedServiceId,
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
        public static async Task Get_EditFlatListPrice_AssociatedServiceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(supplierId, associatedService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_PriceNotFound(
            int supplierId,
            AssociatedService associatedService,
            int cataloguePriceId,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(supplierId, associatedService.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_ReturnsViewWithModel(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            associatedService.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditFlatListPriceModel(associatedService.CatalogueItem, price)
            {
                PracticeReorganisation = PracticeReorganisationTypeEnum.None,
            };

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var result = (await controller.EditFlatListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            int supplierId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            AddEditFlatListPriceModel model,
            AssociatedServiceListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditFlatListPrice(supplierId, associatedServiceId, cataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_SamePublicationStatus(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            associatedService.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    associatedService.CatalogueItemId,
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
                    associatedService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_Redirects(
            int supplierId,
            AssociatedService associatedService,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServiceListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(supplierId, associatedService.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    associatedService.CatalogueItemId,
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
                    associatedService.CatalogueItemId,
                    price.CataloguePriceId,
                    model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }
    }
}
