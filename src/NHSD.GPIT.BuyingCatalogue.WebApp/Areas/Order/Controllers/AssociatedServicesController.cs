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
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services")]
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

        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, internalOrgId, CatalogueItemType.AssociatedService);

            var model = new AssociatedServiceModel(internalOrgId, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var organisation = await organisationService.GetOrganisationByInternalIdentifier(internalOrgId);

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.AssociatedService, null, new OrderItemRecipientModel { OdsCode = internalOrgId, Name = organisation.Name });

            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(order.SupplierId);

            if (!associatedServices.Any())
            {
                return View("NoAssociatedServicesFound", new NoAssociatedServicesFoundModel()
                {
                    BackLink = Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),
                });
            }

            var model = new SelectAssociatedServiceModel(internalOrgId, callOffId, associatedServices, state.CatalogueItemId)
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string internalOrgId, CallOffId callOffId, SelectAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, internalOrgId, model.SelectedSolutionId.Value);

            if (existingOrder != null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { internalOrgId, callOffId, existingOrder.CatalogueItemId });
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
                    new { internalOrgId, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectAssociatedServicePrice),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.Value);

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var model = new SelectAssociatedServicePriceModel(internalOrgId, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectAssociatedService), new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string internalOrgId, CallOffId callOffId, SelectAssociatedServicePriceModel model)
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
                new { internalOrgId, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(internalOrgId, callOffId, catalogueItemId);

            return View(new EditAssociatedServiceModel(internalOrgId, state));
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string internalOrgId, CallOffId callOffId, string catalogueItemId, EditAssociatedServiceModel model)
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

            await orderItemService.Create(callOffId, internalOrgId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
