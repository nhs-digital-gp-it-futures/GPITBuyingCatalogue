using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date")]
    public sealed class AdditionalServiceRecipientsDateController : Controller
    {
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        public AdditionalServiceRecipientsDateController(
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        [HttpGet]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault());

            var model = new SelectAdditionalServiceRecipientsDateModel(state, defaultDeliveryDate)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                    typeof(AdditionalServiceRecipientsController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SelectAdditionalServiceRecipientsDate(string odsCode, CallOffId callOffId, SelectAdditionalServiceRecipientsDateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.PlannedDeliveryDate = model.DeliveryDate;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault(), model.DeliveryDate!.Value);

            orderSessionService.SetOrderStateToSession(state);

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.Declarative)
            {
                return RedirectToAction(
                    nameof(AdditionalServicesController.SelectFlatDeclarativeQuantity),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId });
            }

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                return RedirectToAction(
                    nameof(AdditionalServicesController.SelectFlatOnDemandQuantity),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(AdditionalServicesController.EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }
    }
}
