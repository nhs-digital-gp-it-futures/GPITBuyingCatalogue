using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Review;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/review-solutions-and-services")]
    public class ReviewSolutionsController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPdfService pdfService;
        private readonly PdfSettings pdfSettings;

        public ReviewSolutionsController(IOrderService orderService, IPdfService pdfService, PdfSettings pdfSettings)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        [HttpGet]
        public async Task<IActionResult> ReviewSolutions(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ReviewSolutionsModel(order, internalOrgId)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var result = pdfService.Convert(OrderSummaryUri(internalOrgId, callOffId));

            var fileName = order.OrderStatus == OrderStatus.Completed
                ? $"order-summary-completed-{callOffId}.pdf"
                : $"order-summary-in-progress-{callOffId}.pdf";

            return File(result, "application/pdf", fileName);
        }

        private Uri OrderSummaryUri(string internalOrgId, CallOffId callOffId)
        {
            var uri = Url.Action(
                nameof(OrderSummaryController.Index),
                typeof(OrderSummaryController).ControllerName(),
                new { internalOrgId, callOffId });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uri = $"{Request.Scheme ?? "https"}://{Request.Host}{uri}";
            }
            else
            {
                uri = pdfSettings.UseSslForPdf
                    ? $"https://localhost{uri}"
                    : $"http://localhost{uri}";
            }

            return new Uri(uri);
        }
    }
}
