using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/additional-services/select/additional-service/price/recipients")]
    public sealed class AdditionalServiceRecipientsController : Controller
    {
        private readonly IGpPracticeCacheService gpPracticeService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderSessionService orderSessionService;

        public AdditionalServiceRecipientsController(
            IGpPracticeCacheService gpPracticeService,
            IOdsService odsService,
            IOrderItemService orderItemService,
            IOrderSessionService orderSessionService)
        {
            this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        [HttpGet]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

                state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();

                var orderItems = await orderItemService.GetOrderItems(callOffId, internalOrgId, CatalogueItemType.Solution);
                var orderItem = orderItems.FirstOrDefault(x => $"{state.CatalogueItemId}".StartsWith($"{x.CatalogueItemId}"));
                var orderItemRecipients = orderItem?.OrderItemRecipients ?? Enumerable.Empty<OrderItemRecipient>();

                state.ServiceRecipients
                    .Where(x => orderItemRecipients.Any(r => r.OdsCode == x.OdsCode))
                    .ForEach(x => x.Selected = true);

                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode));
        }

        [HttpPost]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectAdditionalServiceRecipientsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.Patient)
            {
                foreach (var serviceRecipient in model.ServiceRecipients)
                {
                    serviceRecipient.Quantity = await gpPracticeService.GetNumberOfPatients(serviceRecipient.OdsCode);
                }
            }

            state.ServiceRecipients = model.ServiceRecipients;

            orderSessionService.SetOrderStateToSession(state);

            if (state.HasHitEditSolution)
            {
                return RedirectToAction(
                    nameof(AdditionalServicesController.EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { internalOrgId, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
