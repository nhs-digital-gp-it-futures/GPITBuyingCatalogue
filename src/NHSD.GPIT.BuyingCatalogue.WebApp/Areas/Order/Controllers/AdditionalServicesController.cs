﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditonalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly ISolutionsService solutionsService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        // TODO: too many dependencies, i.e. too many responsibilities
        public AdditionalServicesController(
            IOrderService orderService,
            IAdditionalServicesService additionalServicesService,
            ISolutionsService solutionsService,
            IOdsService odsService,
            IOrderItemService orderItemService,
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AdditionalService);

            return View(new AdditionalServiceModel(odsCode, order, orderItems));
        }

        [HttpGet("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            // Get Catalogue Solutions related to the order
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);
            var solutionIds = orderItems.Select(i => i.CatalogueItemId).ToList();

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.AdditionalService, solutionIds, null);

            // Get Additional Services that are related to any Catalogue Solution in the order
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionIds(solutionIds);

            return !additionalServices.Any()
                ? View("NoAdditionalServicesFound", new NoAdditionalServicesFoundModel(odsCode, callOffId))
                : View(new SelectAdditionalServiceModel(odsCode, callOffId, additionalServices, state.CatalogueItemId));
        }

        [HttpPost("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, CallOffId callOffId, SelectAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await additionalServicesService.GetAdditionalServicesBySolutionIds(state.SolutionIds);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedAdditionalServiceId.GetValueOrDefault());

            if (existingOrder is not null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedAdditionalServiceId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (!prices.Any())
                throw new InvalidOperationException($"Additional Service {state.CatalogueItemId.GetValueOrDefault()} does not have any Flat prices associated");

            if (prices.Count == 1)
            {
                state = orderSessionService.SetPrice(callOffId, prices.Single());

                state.SkipPriceSelection = true;
                orderSessionService.SetOrderStateToSession(state);

                return RedirectToAction(
                    nameof(SelectAdditionalServiceRecipients),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(SelectAdditionalServicePrice),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectAdditionalServicePriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        [HttpPost("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, CallOffId callOffId, SelectAdditionalServicePriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(p => p.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(callOffId, cataloguePrice);

            return RedirectToAction(
                nameof(SelectAdditionalServiceRecipients),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/additional-service/price/recipients")]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(string odsCode, CallOffId callOffId, string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectAdditionalServiceRecipientsModel(
                odsCode,
                callOffId,
                state.CatalogueItemName,
                state.ServiceRecipients,
                selectionMode,
                state.IsNewSolution,
                state.CatalogueItemId.GetValueOrDefault()));
        }

        [HttpPost("select/additional-service/price/recipients")]
        public IActionResult SelectAdditionalServiceRecipients(string odsCode, CallOffId callOffId, SelectAdditionalServiceRecipientsModel model)
        {
            if (!model.ServiceRecipients.Any(sr => sr.Selected))
                ModelState.AddModelError("ServiceRecipients[0].Selected", "Select a Service Recipient");

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
                return View(model);

            state.ServiceRecipients = model.ServiceRecipients;

            orderSessionService.SetOrderStateToSession(state);

            if (!state.IsNewSolution)
            {
                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/additional-service/price/recipients/date")]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault());

            return View(new SelectAdditionalServiceRecipientsDateModel(odsCode, state, defaultDeliveryDate));
        }

        [HttpPost("select/additional-service/price/recipients/date")]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, CallOffId callOffId, SelectAdditionalServiceRecipientsDateModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            (DateTime? date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault(), date.Value);

            orderSessionService.SetOrderStateToSession(state);

            if (state.ProvisioningType == ProvisioningType.Declarative)
            {
                return RedirectToAction(
                    nameof(SelectFlatDeclarativeQuantity),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId });
            }

            if (state.ProvisioningType == ProvisioningType.OnDemand)
            {
                return RedirectToAction(
                    nameof(SelectFlatOnDemandQuantity),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity));
        }

        [HttpPost("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            return View(new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod));
        }

        [HttpPost("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError(nameof(model.Quantity), error);

            if (model.EstimationPeriod is null)
                ModelState.AddModelError(nameof(model.EstimationPeriod), "Estimation Period is Required");

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;
            state.EstimationPeriod = model.EstimationPeriod.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, catalogueItemId);

            return View(new EditAdditionalServiceModel(odsCode, callOffId, state));
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, EditAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            for (int i = 0; i < model.OrderItem.ServiceRecipients.Count; i++)
            {
                (DateTime? _, var error) = model.OrderItem.ServiceRecipients[i].ToDateTime(state.CommencementDate);

                if (error != null)
                {
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Day", error);
                }

                if (!model.OrderItem.ServiceRecipients[i].Quantity.HasValue
                    || model.OrderItem.ServiceRecipients[i].Quantity.Value == 0)
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Quantity", "Quantity is Required");
            }

            if (model.OrderItem.AgreedPrice > state.CataloguePrice)
                ModelState.AddModelError("OrderItem.Price", "Price cannot be greater than list price");

            if (!ModelState.IsValid)
            {
                model.UpdateModel(state);
                return View(model);
            }

            state.AgreedPrice = model.OrderItem.AgreedPrice;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            // TODO - handle errors
            var result = await orderItemService.Create(callOffId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteAdditionalServiceModel(odsCode, callOffId, catalogueItemId, catalogueItemName, order.Description));
        }

        [HttpPost("delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName, DeleteAdditionalServiceModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
