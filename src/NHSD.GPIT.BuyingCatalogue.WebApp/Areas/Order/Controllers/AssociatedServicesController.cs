using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/associated-services")]
    public sealed class AssociatedServicesController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly ISolutionsService solutionsService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderSessionService orderSessionService;
        private readonly IOrganisationsService organisationService;

        public AssociatedServicesController(
            IOrderService orderService,
            IAssociatedServicesService associatedServicesService,
            ISolutionsService solutionsService,
            IOrderItemService orderItemService,
            IOrderSessionService orderSessionService,
            IOrganisationsService organisationService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrderThin(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AssociatedService);

            var model = new AssociatedServiceModel(odsCode, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId);

            var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.AssociatedService, null, new OrderItemRecipientModel { OdsCode = odsCode, Name = organisation.Name });

            var associatedServices = await associatedServicesService.GetAssociatedServicesForSupplier(order.SupplierId);

            if (!associatedServices.Any())
            {
                return View("NoAssociatedServicesFound", new NoAssociatedServicesFoundModel()
                {
                    BackLink = Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { odsCode, callOffId }),
                });
            }

            var model = new SelectAssociatedServiceModel(odsCode, callOffId, associatedServices, state.CatalogueItemId)
            {
                BackLink = Url.Action(nameof(Index), new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId, SelectAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await associatedServicesService.GetAssociatedServicesForSupplier(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId.Value);

            if (existingOrder != null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { odsCode, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.Value);
            state.CatalogueItemName = solution.Name;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            if (!prices.Any())
                throw new InvalidOperationException($"Associated Service {state.CatalogueItemId.GetValueOrDefault()} does not have any Flat prices associated");

            if (prices.Count == 1)
            {
                state = orderSessionService.SetPrice(callOffId, prices.Single());

                state.SkipPriceSelection = true;
                orderSessionService.SetOrderStateToSession(state);

                return RedirectToAction(
                    nameof(EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { odsCode, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectAssociatedServicePrice),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.Value);

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var model = new SelectAssociatedServicePriceModel(odsCode, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectAssociatedService), new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId, SelectAssociatedServicePriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.Value);

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(p => p.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(callOffId, cataloguePrice);

            return RedirectToAction(
                nameof(EditAssociatedService),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, catalogueItemId);

            return View(new EditAssociatedServiceModel(odsCode, state));
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, string catalogueItemId, EditAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                state.EstimationPeriod = model.EstimationPeriod;
            }

            if (!ModelState.IsValid)
            {
                model.UpdateModel(state);
                return View(model);
            }

            state.AgreedPrice = model.OrderItem.AgreedPrice;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            await orderItemService.Create(callOffId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
