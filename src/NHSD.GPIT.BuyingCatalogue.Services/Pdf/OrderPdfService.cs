using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf;

public class OrderPdfService : IOrderPdfService
{
    private const string ActionName = "Index";
    private const string ControllerName = "OrderSummary";

    private readonly IPdfService pdfService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public OrderPdfService(
        IPdfService pdfService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        this.httpContextAccessor =
            httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<MemoryStream> CreateOrderSummaryPdf(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var url = OrderSummaryUri(order.OrderingParty.InternalIdentifier, order.CallOffId);

        var pdfContents = await pdfService.Convert(url);

        return new(pdfContents);
    }

    [ExcludeFromCodeCoverage(Justification = "Code paths are OS dependent and can't be reliably tested.")]
    private Uri OrderSummaryUri(string internalOrgId, CallOffId callOffId)
    {
        var uri = httpContextAccessor.HttpContext.GetUrlHelper()
            .Action(
                ActionName,
                ControllerName,
                new { internalOrgId, callOffId });

        return new(pdfService.BaseUri(), uri);
    }
}
