using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/manage-orders")]
    public class ManageOrdersController : Controller
    {
        private readonly IOrderAdminService orderAdminService;
        private readonly ICsvService csvService;
        private readonly IOrderService orderService;
        private readonly IOrderPdfService pdfService;
        private readonly PdfSettings pdfSettings;

        public ManageOrdersController(
            IOrderAdminService orderAdminService,
            ICsvService csvService,
            IOrderService orderService,
            IOrderPdfService pdfService,
            PdfSettings pdfSettings)
        {
            this.orderAdminService = orderAdminService ?? throw new ArgumentNullException(nameof(orderAdminService));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page = "",
            [FromQuery] string search = "",
            [FromQuery] string searchTermType = "")
        {
            const int pageSize = 10;
            var pageOptions = new PageOptions(page, pageSize);

            var orders = await orderAdminService.GetPagedOrders(pageOptions, search, searchTermType);

            var model = new ManageOrdersDashboardModel(orders.Items, orders.Options)
            {
                BackLink = Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpGet("search-suggestions")]
        public async Task<IActionResult> FilterSearchSuggestions(
            [FromQuery] string search = "")
        {
            var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());

            var results = await orderAdminService.GetOrdersBySearchTerm(search);

            return Json(results.Select(r =>
                new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).AppendQueryParameterToUrl("searchTermType", r.Category).Uri.PathAndQuery)));
        }

        [HttpGet("{callOffId}")]
        public async Task<IActionResult> ViewOrder(CallOffId callOffId, string returnUrl = null)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var model = new ViewOrderModel(order)
            {
                BackLink = returnUrl ?? Url.Action(nameof(Index)),
            };

            return View(model);
        }

        [HttpGet("{callOffId}/download/full-order-csv")]
        public async Task<IActionResult> DownloadFullOrderCsv(CallOffId callOffId, string externalOrgId)
        {
            var order = await orderAdminService.GetOrder(callOffId);
            using var memoryStream = new MemoryStream();
            await csvService.CreateFullOrderCsvAsync(order.Id, order.OrderType, memoryStream);
            memoryStream.Position = 0;
            var callOffIdPrefix = GetFileNamePrefix(callOffId);

            return File(memoryStream.ToArray(), "application/octet-stream", $"{callOffIdPrefix}{externalOrgId}_full.csv");
        }

        [HttpGet("{callOffId}/download/pdf")]
        public async Task<IActionResult> Download(CallOffId callOffId, string internalOrgId)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var result = await pdfService.CreateOrderSummaryPdf(order);

            var fileName = order.OrderStatus == OrderStatus.Completed
                ? $"order-summary-completed-{callOffId}.pdf"
                : $"order-summary-in-progress-{callOffId}.pdf";

            return File(result.ToArray(), "application/pdf", fileName);
        }

        [HttpGet("{callOffId}/delete/not-latest")]
        public async Task<IActionResult> DeleteNotLatest(CallOffId callOffId)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var model = new DeleteNotLatestModel(order.CallOffId)
            {
                BackLink = Url.Action(nameof(ViewOrder), new { callOffId }),
            };

            return View(model);
        }

        [HttpGet("{callOffId}/delete")]
        public async Task<IActionResult> DeleteOrder(CallOffId callOffId)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(order.CallOffId);
            if (hasSubsequentRevisions)
            {
                return RedirectToAction(
                    nameof(ManageOrdersController.DeleteNotLatest),
                    new { callOffId });
            }

            var model = new DeleteOrderModel(order.CallOffId)
            {
                BackLink = Url.Action(nameof(ViewOrder), new { callOffId }),
                OrderCreationDate = order.Created,
            };

            return View(model);
        }

        [HttpPost("{callOffId}/delete")]
        public async Task<IActionResult> DeleteOrder(DeleteOrderModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderAdminService.DeleteOrder(model.CallOffId, model.NameOfRequester, model.NameOfApprover, model.ApprovalDate);

            return RedirectToAction(nameof(Index));
        }

        private static string GetFileNamePrefix(CallOffId callOffId)
        {
            return callOffId.IsAmendment
                ? $"Amendment_{callOffId}_"
                : $"{callOffId}_";
        }
    }
}
