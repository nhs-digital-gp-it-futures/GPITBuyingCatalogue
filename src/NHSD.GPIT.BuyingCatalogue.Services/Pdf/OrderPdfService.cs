using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf;

public class OrderPdfService : IOrderPdfService
{
    private const string ActionName = "Index";
    private const string ControllerName = "OrderSummary";

    private readonly IPdfService pdfService;
    private readonly IUrlHelperFactory urlHelper;
    private readonly IActionContextAccessor actionContextAccessor;

    public OrderPdfService(
        IPdfService pdfService,
        IUrlHelperFactory urlHelper,
        IActionContextAccessor actionContextAccessor)
    {
        this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        this.urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        this.actionContextAccessor =
            actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
    }

    public async Task<MemoryStream> CreateOrderSummaryPdf(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var url = OrderSummaryUri(order.OrderingParty.InternalIdentifier, order.CallOffId);

        var pdfContents = await pdfService.Convert(url);

        return new(pdfContents);
    }

    [ExcludeFromCodeCoverage(Justification = "Code paths are OS dependent and can't be reliably tested.")]
    private Uri OrderSummaryUri(string internalOrgId, CallOffId callOffId)
    {
        var uri = urlHelper.GetUrlHelper(actionContextAccessor.ActionContext!)
            .Action(
                ActionName,
                ControllerName,
                new { internalOrgId, callOffId });

        return new(pdfService.BaseUri(), uri);
    }
}
