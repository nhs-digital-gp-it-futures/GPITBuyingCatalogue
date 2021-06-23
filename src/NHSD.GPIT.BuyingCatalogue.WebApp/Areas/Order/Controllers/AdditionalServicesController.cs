﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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
        private readonly ILogWrapper<AdditionalServicesController> logger;
        private readonly IOrderService orderService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly ISessionService sessionService;
        private readonly ISolutionsService solutionsService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        public AdditionalServicesController(
            ILogWrapper<AdditionalServicesController> logger,
            IOrderService orderService,
            IAdditionalServicesService additionalServicesService,
            ISessionService sessionService,
            ISolutionsService solutionsService,
            IOdsService odsService,
            IOrderItemService orderItemService,
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        // TODO: callOffId should be of type CallOffId
        public async Task<IActionResult> Index(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(Index)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            sessionService.ClearSession();

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AdditionalService);

            return View(new AdditionalServiceModel(odsCode, order, orderItems));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);

            var solutionIds = orderItems.Select(x => x.CatalogueItemId.ToString());

            var state = new CreateOrderItemModel
            {
                IsNewOrder = true,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = CatalogueItemType.AdditionalService,
                SolutionIds = solutionIds,
            };

            orderSessionService.SetOrderStateToSession(state);

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionIds(solutionIds);

            return !additionalServices.Any()
                ? View("NoAdditionalServicesFound", new NoAdditionalServicesFoundModel(odsCode, callOffId))
                : View(new SelectAdditionalServiceModel(odsCode, callOffId, additionalServices, state.CatalogueItemId));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, string callOffId, SelectAdditionalServiceModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
            {
                model.Solutions = await additionalServicesService.GetAdditionalServicesBySolutionIds(state.SolutionIds);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId);

            if (existingOrder is not null)
            {
                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, id = existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolution(state.CatalogueItemId);
            state.CatalogueItemName = solution.Name;

            var additionalService = await additionalServicesService.GetAdditionalService(model.SelectedSolutionId);
            state.CatalogueSolutionId = additionalService.SolutionId;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (prices.Count == 1)
            {
                orderSessionService.SetPrice(prices.Single());

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

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServicePrice)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectAdditionalServicePriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, string callOffId, SelectAdditionalServicePriceModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServicePrice)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId);

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(x => x.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(cataloguePrice);

            return RedirectToAction(
                nameof(SelectAdditionalServiceRecipients),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service/price/recipients")]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(string odsCode, string callOffId, string selectionMode)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(x => new OrderItemRecipientModel(x)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectAdditionalServiceRecipientsModel(odsCode, callOffId, state.CatalogueItemName, state.ServiceRecipients, selectionMode, state.IsNewOrder, state.CatalogueItemId));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service/price/recipients")]
        public IActionResult SelectAdditionalServiceRecipients(string odsCode, string callOffId, SelectAdditionalServiceRecipientsModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            if (!model.ServiceRecipients.Any(sr => sr.Selected))
                ModelState.AddModelError("ServiceRecipients[0].Selected", "Select a Service Recipient");

            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
                return View(model);

            state.ServiceRecipients = model.ServiceRecipients;

            orderSessionService.SetOrderStateToSession(state);

            if (!state.IsNewOrder)
            {
                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, id = state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service/price/recipients/date")]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, state.CatalogueItemId);

            return View(new SelectAdditionalServiceRecipientsDateModel(odsCode, callOffId, state.CatalogueItemName, state.CommencementDate, state.PlannedDeliveryDate, defaultDeliveryDate));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service/price/recipients/date")]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, string callOffId, SelectAdditionalServiceRecipientsDateModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectAdditionalServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            (DateTime? date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, state.CatalogueItemId, date.Value);

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
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession();

            state.Quantity = quantity;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(SelectFlatOnDemandQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.TimeUnit));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, string callOffId, SelectFlatOnDemandQuantityModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError(nameof(model.Quantity), error);

            if (model.TimeUnit is null)
                ModelState.AddModelError(nameof(model.TimeUnit), "Time Unit is Required");

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession();

            state.Quantity = quantity;
            state.TimeUnit = model.TimeUnit.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: id should be renamed to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("{id}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, string callOffId, string id)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(EditAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var isNewSolution = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, id);

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new EditAdditionalServiceModel(odsCode, callOffId, id, state, isNewSolution));
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: id should be renamed to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpPost("{id}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, string callOffId, string id, EditAdditionalServiceModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(EditAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            for (int i = 0; i < model.OrderItem.ServiceRecipients.Count; i++)
            {
                (DateTime? date, var error) = model.OrderItem.ServiceRecipients[i].ToDateTime(state.CommencementDate);

                if (error != null)
                {
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Day", error);
                }

                if (!model.OrderItem.ServiceRecipients[i].Quantity.HasValue
                    || model.OrderItem.ServiceRecipients[i].Quantity.Value == 0)
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Quantity", "Quantity is Required");
            }

            var solutionListPrices = await solutionsService.GetSolutionListPrices(state.CatalogueItemId);

            var solutionPrice = solutionListPrices.CataloguePrices.Where(cp =>
                cp.ProvisioningType == state.ProvisioningType
                && cp.CataloguePriceType == state.Type
                && (cp.TimeUnit is null || cp.TimeUnit == state.TimeUnit)).FirstOrDefault();

            if (solutionPrice is not null)
            {
                if (model.OrderItem.Price > solutionPrice.Price)
                    ModelState.AddModelError("OrderItem.Price", "Price cannot be greater than list price");
            }

            if (!ModelState.IsValid)
            {
                model.OrderItem.ItemUnit = state.ItemUnit;
                model.OrderItem.TimeUnit = state.TimeUnit;
                return View(model);
            }

            state.Price = model.OrderItem.Price;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            // TODO - handle errors
            var result = await orderItemService.Create(callOffId, state);

            sessionService.ClearSession();

            return RedirectToAction(
                nameof(Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: id should be renamed to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("delete/{id}/confirmation/{serviceName}")]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, string callOffId, string id, string serviceName)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(DeleteAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(serviceName)} {serviceName}");

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteAdditionalServiceModel(odsCode, callOffId, id, serviceName, order.Description));
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: id should be renamed to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpPost("delete/{id}/confirmation/{serviceName}")]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, string callOffId, string id, string serviceName, DeleteAdditionalServiceModel model)
        {
            logger.LogInformation($"Handling post for {nameof(AdditionalServicesController)}.{nameof(DeleteAdditionalService)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(serviceName)} {serviceName}");

            await orderItemService.DeleteOrderItem(callOffId, id);

            return RedirectToAction(
                nameof(DeleteContinue),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, id, serviceName });
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: id should be renamed to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("delete/{id}/confirmation/{serviceName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string serviceName)
        {
            logger.LogInformation($"Taking user to {nameof(AdditionalServicesController)}.{nameof(DeleteContinue)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(serviceName)} {serviceName}");

            return View(new DeleteContinueModel(odsCode, callOffId, serviceName));
        }
    }
}
