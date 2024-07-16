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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionListPriceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithModel(
            Solution solution,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new ManageListPricesModel(solution.CatalogueItem, solution.CatalogueItem.CataloguePrices);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.Index(solution.CatalogueItemId)).As<ViewResult>();

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
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.Index(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_ReturnsViewWithModel(
            Solution solution,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new ListPriceTypeModel(solution.CatalogueItem);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.ListPriceType(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ListPriceType_SolutionNotFound(
            CatalogueItemId solutionId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.ListPriceType(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static void Post_ListPriceType_Flat_RedirectsCorrectly(
            Solution solution,
            ListPriceTypeModel model,
            CatalogueSolutionListPriceController controller)
        {
            model.SelectedCataloguePriceType = CataloguePriceType.Flat;

            var result = controller.ListPriceType(solution.CatalogueItemId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.AddFlatListPrice));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_ReturnsViewWithModel(
            Solution solution,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new AddTieredListPriceModel(solution.CatalogueItem);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredListPrice_WithPriceId_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice cataloguePrice,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(cataloguePrice);

            var model = new AddTieredListPriceModel(solution.CatalogueItem, cataloguePrice);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, cataloguePrice.CataloguePriceId)).As<ViewResult>();

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
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredListPrice(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_AddTieredListPrice_Redirects(
            Solution solution,
            AddTieredListPriceModel model,
            CatalogueSolutionListPriceController controller)
        {
            var result = (await controller.AddTieredListPrice(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new TieredPriceTiersModel(solution.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.TieredPriceTiers(solutionId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TieredPriceTiers_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_InvalidModel_ReturnsView(
            Solution solution,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            model.Tiers = price.CataloguePriceTiers.ToList();

            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TieredPriceTiers_Redirects(
            Solution solution,
            CataloguePrice price,
            TieredPriceTiersModel model,
            CatalogueSolutionListPriceController controller)
        {
            var result = (await controller.TieredPriceTiers(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(solution.CatalogueItem, price)
            {
                IsEditing = false,
            };

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsEditing_CorrectBacklink(
            Solution solution,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, true)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_IsNotEditing_CorrectBacklink(
            Solution solution,
            CataloguePrice price,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, false)).As<ViewResult>();

            urlHelper.Received().Action(Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_SolutionNotFound(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AddTieredPriceTier(solutionId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddTieredPriceTier_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, cataloguePriceId, false)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_InvalidModel_ReturnsView(
            Solution solution,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddTieredPriceTier_Redirects(
            Solution solution,
            CataloguePrice price,
            AddEditTieredPriceTierModel model,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = false;

            var result = (await controller.AddTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "solutionId", solution.CatalogueItemId },
                { "cataloguePriceId", price.CataloguePriceId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredListPrice(solutionId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredListPrice_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] PriceTiersCapSettings priceTiersSetting,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditTieredListPriceModel(solution.CatalogueItem, price, priceTiersSetting.MaximumNumberOfPriceTiers);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.Tiers = price.CataloguePriceTiers.ToList();

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_SamePublicationStatus(
            Solution solution,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            solution.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    solution.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            await listPriceService.DidNotReceive().SetPublicationStatus(
                        solution.CatalogueItemId,
                        price.CataloguePriceId,
                        model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredListPrice_Redirects(
            Solution solution,
            CataloguePrice price,
            EditTieredListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            solution.CatalogueItem.CataloguePrices.Add(price);
            model.SelectedPublicationStatus = PublicationStatus.Published;

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditTieredListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    solution.CatalogueItemId,
                    price.CataloguePriceId,
                    Arg.Is<PricingUnit>(match => match.Description == pricingUnit.Description
                        && match.Definition == pricingUnit.Definition
                        && match.RangeDescription == pricingUnit.RangeDescription),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            await listPriceService.Received().SetPublicationStatus(
                        solution.CatalogueItemId,
                        price.CataloguePriceId,
                        model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_SolutionNotFound(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.EditTieredPriceTier(solutionId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            int cataloguePriceTierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, cataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_PriceTierNotFound(
            Solution solution,
            CataloguePrice price,
            int cataloguePriceTierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, cataloguePriceTierId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsEditing_CorrectBacklink(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.EditTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, true);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.EditTieredListPrice)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_IsNotEditing_CorrectBacklink(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IUrlHelper urlHelper,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.EditTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, false);

            urlHelper.Received().Action(
                Arg.Is<UrlActionContext>(match => match.Action == nameof(controller.TieredPriceTiers)));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditTieredPriceTierModel(solution.CatalogueItem, price, tier);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id)).As<ViewResult>();

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
            AddEditTieredPriceTierModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTieredPriceTier(model.CatalogueItemId, model.CataloguePriceId, model.TierId!.Value, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTieredPriceTier_IsInfiniteRange(
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsInfiniteRange = true;

            _ = await controller.EditTieredPriceTier(model.CatalogueItemId, model.CataloguePriceId, model.TierId!.Value, model);

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
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsInfiniteRange = false;

            _ = await controller.EditTieredPriceTier(model.CatalogueItemId, model.CataloguePriceId, model.TierId!.Value, model);

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
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = true;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(model.CatalogueItemId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

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
            AddEditTieredPriceTierModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";
            model.IsEditing = false;
            model.IsInfiniteRange = true;

            var result = (await controller.EditTieredPriceTier(model.CatalogueItemId, model.CataloguePriceId, model.TierId!.Value, model)).As<RedirectToActionResult>();

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
        public static async Task Get_EditTierPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.EditTierPrice(solutionId, cataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            int tierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, cataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_TierNotFound(
            Solution solution,
            CataloguePrice price,
            int tierId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, price.CataloguePriceId, tierId, 0)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditTierPrice_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new EditTierPriceModel(solution.CatalogueItem, price, tier)
            {
                TierIndex = 0,
            };

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditTierPrice(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, model.TierIndex)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_InvalidModel_ReturnsView(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditTierPrice(solutionId, cataloguePriceId, tierId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditTierPrice_Redirects(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model,
            [Frozen] IListPriceService service,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.EditTierPrice(solutionId, cataloguePriceId, tierId, model)).As<RedirectToActionResult>();

            await service.Received().UpdateTierPrice(solutionId, cataloguePriceId, tierId, model.Price!.Value);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteListPrice_ReturnsViewWithModel(
            Solution solution,
            int cataloguePriceId,
            [Frozen] IListPriceService service,
            CatalogueSolutionListPriceController controller)
        {
            service.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                solution.CatalogueItem.Name,
                "This list price will be deleted");

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, cataloguePriceId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_Redirects(
            Solution solution,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeleteListPrice(solution.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteListPrice_PublishedPrice_DoesNotDelete(
            Solution solution,
            CataloguePrice price,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice> { price };

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.DeleteListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeleteListPrice(solution.CatalogueItemId, price.CataloguePriceId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteTieredPriceTier_ReturnsViewWithModel(
            Solution solution,
            int cataloguePriceId,
            int tierId,
            [Frozen] IListPriceService service,
            CatalogueSolutionListPriceController controller)
        {
            service.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                solution.CatalogueItem.Name,
                "This pricing tier will be deleted");

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, cataloguePriceId, tierId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_Redirects(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.TieredPriceTiers));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_PublishedPrice_DoesNotDelete(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, model)).As<RedirectToActionResult>();

            await listPriceService.DidNotReceive().DeletePriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteTieredPriceTier_IsEditing_Redirects(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            DeleteItemConfirmationModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Unpublished;
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices.Add(price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.DeleteTieredPriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id, model, true)).As<RedirectToActionResult>();

            await listPriceService.Received().DeletePriceTier(solution.CatalogueItemId, price.CataloguePriceId, tier.Id);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditTieredListPrice));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AddFlatListPrice(solutionId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddFlatListPrice_ReturnsViewWithModel(
            Solution solution,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            var model = new AddEditFlatListPriceModel(solution.CatalogueItem);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.AddFlatListPrice(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_InvalidModel(
            CatalogueItemId solutionId,
            AddEditFlatListPriceModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddFlatListPrice(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddFlatListPrice_Redirects(
            CatalogueItemId solutionId,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            model.InputPrice = "3.14";

            var result = (await controller.AddFlatListPrice(solutionId, model)).As<RedirectToActionResult>();

            var pricingUnit = model.GetPricingUnit();
            await listPriceService.Received().AddListPrice(
                    solutionId,
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
        public static async Task Get_EditFlatListPrice_SolutionNotFound(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.EditFlatListPrice(solutionId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_PriceNotFound(
            Solution solution,
            int cataloguePriceId,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, cataloguePriceId)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditFlatListPrice_ReturnsViewWithModel(
            Solution solution,
            CataloguePrice price,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            solution.CatalogueItem.CataloguePrices.Add(price);

            var model = new AddEditFlatListPriceModel(solution.CatalogueItem, price);

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, price.CataloguePriceId)).As<ViewResult>();

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
            int cataloguePriceId,
            AddEditFlatListPriceModel model,
            CatalogueSolutionListPriceController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditFlatListPrice(solutionId, cataloguePriceId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_SamePublicationStatus(
            Solution solution,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Published;
            solution.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    solution.CatalogueItemId,
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
                        solution.CatalogueItemId,
                        price.CataloguePriceId,
                        model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditFlatListPrice_Redirects(
            Solution solution,
            CataloguePrice price,
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService listPriceService,
            CatalogueSolutionListPriceController controller)
        {
            price.PublishedStatus = PublicationStatus.Draft;
            solution.CatalogueItem.CataloguePrices.Add(price);
            model.InputPrice = "3.14";
            model.SelectedPublicationStatus = PublicationStatus.Published;

            listPriceService.GetCatalogueItemWithListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var pricingUnit = model.GetPricingUnit();

            var result = (await controller.EditFlatListPrice(solution.CatalogueItemId, price.CataloguePriceId, model)).As<RedirectToActionResult>();

            await listPriceService.Received().UpdateListPrice(
                    solution.CatalogueItemId,
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
                        solution.CatalogueItemId,
                        price.CataloguePriceId,
                        model.SelectedPublicationStatus!.Value);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }
    }
}
