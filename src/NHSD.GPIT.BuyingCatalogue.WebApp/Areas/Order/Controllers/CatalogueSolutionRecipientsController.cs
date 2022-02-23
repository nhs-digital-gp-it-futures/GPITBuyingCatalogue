using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients")]
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
        public async Task<IActionResult> SelectSolutionServiceRecipients(string internalOrgId, CallOffId callOffId, string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);
                state.ServiceRecipients = recipients.Select(sr => new OrderItemRecipientModel(sr)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(internalOrgId, state, selectionMode));
        }

        [HttpPost]
        public IActionResult SelectSolutionServiceRecipients(string internalOrgId, CallOffId callOffId, SelectSolutionServiceRecipientsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);
            state.ServiceRecipients = model.ServiceRecipients;

            orderSessionService.SetOrderStateToSession(state);

            if (state.HasHitEditSolution)
            {
                return RedirectToAction(
                    nameof(CatalogueSolutionsController.EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
