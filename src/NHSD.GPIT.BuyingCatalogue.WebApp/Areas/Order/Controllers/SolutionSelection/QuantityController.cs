using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class QuantityController : Controller
    {
        private const string OrderItemViewName = "SelectOrderItemQuantity";
        private const string ServiceRecipientViewName = "SelectServiceRecipientQuantity";

        private readonly IGpPracticeCacheService gpPracticeCache;
        private readonly IOrderService orderService;
        private readonly IOrderPriceService orderPriceService;

        public QuantityController(
            IGpPracticeCacheService gpPracticeCache,
            IOrderService orderService,
            IOrderPriceService orderPriceService)
        {
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderPriceService = orderPriceService ?? throw new ArgumentNullException(nameof(orderPriceService));
        }

        [HttpGet("quantity/select")]
        public async Task<IActionResult> SelectQuantity(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var solution = order.GetSolution();

            if (solution.OrderItemPrice.ProvisioningType == ProvisioningType.Patient)
            {
                return RedirectToAction(
                    nameof(SelectServiceRecipientQuantity),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = new SelectOrderItemQuantityModel
            {
                BackLink = Url.Action(
                    nameof(PricesController.EditPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ItemName = solution.CatalogueItem.Name,
                ItemType = solution.CatalogueItem.CatalogueItemType.Name(),
            };

            return View(OrderItemViewName, model);
        }

        [HttpPost("quantity/select")]
        public async Task<IActionResult> SelectQuantity(string internalOrgId, CallOffId callOffId, SelectOrderItemQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(OrderItemViewName, model);
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            await orderPriceService.SetOrderItemQuantity(
                order.Id,
                order.GetSolution().CatalogueItemId,
                int.Parse(model.Quantity));

            return GetRedirect(internalOrgId, callOffId, order);
        }

        [HttpGet("quantity/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var solution = order.GetSolution();

            var model = new SelectServiceRecipientQuantityModel(solution)
            {
                BackLink = Url.Action(
                    nameof(PricesController.EditPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var quantity = await gpPracticeCache.GetNumberOfPatients(serviceRecipient.OdsCode);

                if (quantity.HasValue)
                {
                    serviceRecipient.Quantity = quantity.Value;
                }
            }

            model.ServiceRecipients = model.ServiceRecipients
                .OrderBy(x => x.Quantity == 0 ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToArray();

            return View(ServiceRecipientViewName, model);
        }

        [HttpPost("quantity/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(string internalOrgId, CallOffId callOffId, SelectServiceRecipientQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ServiceRecipientViewName, model);
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var catalogueItemId = order.GetSolution().CatalogueItemId;
            var quantities = model.ServiceRecipients
                .Select(x => new OrderPricingTierQuantityDto
                {
                    OdsCode = x.OdsCode,
                    Quantity = x.Quantity > 0
                        ? x.Quantity
                        : int.Parse(x.InputQuantity),
                })
                .ToList();

            await orderPriceService.SetServiceRecipientQuantities(order.Id, catalogueItemId, quantities);

            return GetRedirect(internalOrgId, callOffId, order);
        }

        private IActionResult GetRedirect(string internalOrgId, CallOffId callOffId, EntityFramework.Ordering.Models.Order order)
        {
            var additionalService = order.GetAdditionalServices().FirstOrDefault(x => x.OrderItemRecipients.Count == 0);

            if (additionalService != null)
            {
                return RedirectToAction(
                    nameof(ServiceRecipientsController.AdditionalServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, additionalService.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(AssociatedServicesController.AddAssociatedServices),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
