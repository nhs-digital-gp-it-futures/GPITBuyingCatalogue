using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients")]
    public sealed class AdditionalServiceRecipientsController : Controller
    {
        private readonly IGpPracticeService gpPracticeService;
        private readonly IOdsService odsService;
        private readonly IOrderSessionService orderSessionService;

        public AdditionalServiceRecipientsController(
            IGpPracticeService gpPracticeService,
            IOdsService odsService,
            IOrderSessionService orderSessionService)
        {
            this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        [HttpGet]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(string odsCode, CallOffId callOffId, string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode));
        }

        [HttpPost]
        public async Task<IActionResult> SelectAdditionalServiceRecipients(string odsCode, CallOffId callOffId, SelectAdditionalServiceRecipientsModel model)
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
                    new { odsCode, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
