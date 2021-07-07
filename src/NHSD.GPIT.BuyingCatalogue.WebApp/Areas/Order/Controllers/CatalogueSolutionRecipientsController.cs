using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients")]
    public sealed class CatalogueSolutionRecipientsController : Controller
    {
        private readonly IOdsService odsService;
        private readonly IOrderSessionService orderSessionService;

        public CatalogueSolutionRecipientsController(
            IOdsService odsService,
            IOrderSessionService orderSessionService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        [HttpGet]
        public async Task<IActionResult> SelectSolutionServiceRecipients(string odsCode, CallOffId callOffId, string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(x => new OrderItemRecipientModel(x)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(
                odsCode,
                callOffId,
                state.CatalogueItemName,
                state.ServiceRecipients,
                selectionMode,
                state.IsNewSolution,
                state.CatalogueItemId.GetValueOrDefault()));
        }

        [HttpPost]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, CallOffId callOffId, SelectSolutionServiceRecipientsModel model)
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
                    nameof(CatalogueSolutionsController.EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
