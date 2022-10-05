using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/manage-orders")]
    public class ManageOrdersController : Controller
    {
        private readonly IFrameworkService frameworkService;
        private readonly IOrderAdminService orderAdminService;
        private readonly ICsvService csvService;
        private readonly IPdfService pdfService;
        private readonly PdfSettings pdfSettings;

        public ManageOrdersController(
            IFrameworkService frameworkService,
            IOrderAdminService orderAdminService,
            ICsvService csvService,
            IPdfService pdfService,
            PdfSettings pdfSettings)
        {
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
            this.orderAdminService = orderAdminService ?? throw new ArgumentNullException(nameof(orderAdminService));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
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
                new SuggestionSearchResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).AppendQueryParameterToUrl("searchTermType", r.Category).ToString(),
                }));
        }

        [HttpGet("{callOffId}")]
        public async Task<IActionResult> ViewOrder(CallOffId callOffId, string returnUrl = null)
        {
            var order = await orderAdminService.GetOrder(callOffId);
            var framework = await frameworkService.GetFramework(order?.Id ?? 0);

            var model = new ViewOrderModel(order, framework)
            {
                BackLink = returnUrl ?? Url.Action(nameof(Index)),
            };

            return View(model);
        }

        [HttpGet("{callOffId}/download/full-order-csv")]
        public async Task<IActionResult> DownloadFullOrderCsv(CallOffId callOffId, string externalOrgId)
        {
            using var memoryStream = new MemoryStream();
            await csvService.CreateFullOrderCsvAsync(callOffId.Id, memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream.ToArray(), "application/octet-stream", $"{callOffId}_{externalOrgId}_full.csv");
        }

        [HttpGet("{callOffId}/download/patient-order-csv")]
        public async Task<IActionResult> DownloadPatientNumberCsv(CallOffId callOffId, string externalOrgId)
        {
            using var memoryStream = new MemoryStream();
            await csvService.CreatePatientNumberCsvAsync(callOffId.Id, memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream.ToArray(), "application/octet-stream", $"{callOffId}_{externalOrgId}_patient.csv");
        }

        [HttpGet("{callOffId}/download/pdf")]
        public async Task<IActionResult> Download(CallOffId callOffId, string internalOrgId)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var result = pdfService.Convert(OrderSummaryUri(internalOrgId, callOffId));

            var fileName = order.OrderStatus == OrderStatus.Completed
                ? $"order-summary-completed-{callOffId}.pdf"
                : $"order-summary-in-progress-{callOffId}.pdf";

            return File(result, "application/pdf", fileName);
        }

        [HttpGet("{callOFfId}/delete")]
        public async Task<IActionResult> DeleteOrder(CallOffId callOffId)
        {
            var order = await orderAdminService.GetOrder(callOffId);

            var model = new DeleteOrderModel(order)
            {
                BackLink = Url.Action(nameof(ViewOrder), new { callOffId }),
                OrderCreationDate = order.Created,
            };

            return View(model);
        }

        [HttpPost("{callOffId}/delete")]
        public async Task<IActionResult> DeleteOrder(CallOffId callOffId, DeleteOrderModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderAdminService.DeleteOrder(callOffId, model.NameOfRequester, model.NameOfApprover, model.ApprovalDate ?? null);
            return RedirectToAction(nameof(Index));
        }

        private Uri OrderSummaryUri(string internalOrgId, CallOffId callOffId)
        {
            var uri = Url.Action(
                nameof(OrderSummaryController.Index),
                typeof(OrderSummaryController).ControllerName(),
                new { internalOrgId, callOffId });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uri = $"{Request.Scheme}://{Request.Host}{uri}";
            }
            else
            {
                uri = pdfSettings.UseSslForPdf
                    ? $"https://localhost{uri}"
                    : $"http://localhost{uri}";
            }

            return new(uri);
        }
    }
}
