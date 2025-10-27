using System;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf;

public class CachedOrderPdfService : IOrderPdfService
{
    private const string OrderPdfContainerName = "orderpdfs";
    private readonly IOrderPdfService orderPdfService;
    private readonly IAzureBlobStorageService azureBlobStorageService;

    public CachedOrderPdfService(
        IOrderPdfService orderPdfService,
        IAzureBlobStorageService azureBlobStorageService)
    {
        this.orderPdfService = orderPdfService ?? throw new ArgumentNullException(nameof(orderPdfService));
        this.azureBlobStorageService =
            azureBlobStorageService ?? throw new ArgumentNullException(nameof(azureBlobStorageService));
    }

    public async Task<MemoryStream> CreateOrderSummaryPdf(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.OrderStatus == OrderStatus.InProgress)
            return await orderPdfService.CreateOrderSummaryPdf(order);

        var callOffId = order.CallOffId.ToString();

        var blobDocument = order.OrderStatus == OrderStatus.Completed ? $"{callOffId}.pdf" : $"{callOffId}-terminated.pdf";

        var cachedPdf = await azureBlobStorageService.DownloadAsync(new BlobDocument(OrderPdfContainerName, blobDocument));
        if (cachedPdf != null)
            return cachedPdf;

        var file = await orderPdfService.CreateOrderSummaryPdf(order);

        await azureBlobStorageService.UploadAsync(new BlobDocument(OrderPdfContainerName, blobDocument), file);

        return file;
    }
}
