using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipientsDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date")]
    public sealed class CatalogueSolutionRecipientsDateController : Controller
    {
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        public CatalogueSolutionRecipientsDateController(
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        [HttpGet]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, odsCode, state.CatalogueItemId.GetValueOrDefault());

            var model = new SelectSolutionServiceRecipientsDateModel(state, defaultDeliveryDate)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                    typeof(CatalogueSolutionRecipientsController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, CallOffId callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);
            state.PlannedDeliveryDate = model.DeliveryDate;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, odsCode, state.CatalogueItemId.GetValueOrDefault(), model.DeliveryDate!.Value);

            orderSessionService.SetOrderStateToSession(state);

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.Declarative)
            {
                return RedirectToAction(
                    nameof(CatalogueSolutionsController.SelectFlatDeclarativeQuantity),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId });
            }

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                return RedirectToAction(
                    nameof(CatalogueSolutionsController.SelectFlatOnDemandQuantity),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(CatalogueSolutionsController.EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }
    }
}
