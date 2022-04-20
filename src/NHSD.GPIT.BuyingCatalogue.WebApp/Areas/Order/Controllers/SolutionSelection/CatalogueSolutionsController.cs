using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.CatalogueSolutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;

        public CatalogueSolutionsController(
            IAdditionalServicesService additionalServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            ISolutionsService solutionsService)
        {
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            if (order.AssociatedServicesOnly)
            {
                return RedirectToAction(
                    nameof(AssociatedServicesController.SelectAssociatedServices),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = await GetSelectModel(internalOrgId, callOffId);

            return View(model);
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await GetSelectModel(internalOrgId, callOffId, model.SelectedCatalogueSolutionId);

                return View(model);
            }

            var solutionId = CatalogueItemId.ParseExact(model.SelectedCatalogueSolutionId);
            var additionalServiceIds = model.GetAdditionalServicesIdsForSelectedCatalogueSolution();
            var ids = additionalServiceIds.Union(new[] { solutionId });

            await orderItemService.AddOrderItems(internalOrgId, callOffId, ids);

            return RedirectToAction(
                nameof(ServiceRecipientsController.SolutionRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private async Task<SelectSolutionModel> GetSelectModel(string internalOrgId, CallOffId callOffId, string selected = null)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solutions = await solutionsService.GetSupplierSolutions(order.SupplierId);
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionIds(solutions.Select(x => x.Id));

            return new SelectSolutionModel(order, solutions, additionalServices)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                SelectedCatalogueSolutionId = selected ?? $"{order.GetSolution()?.CatalogueItemId}",
            };
        }
    }
}
