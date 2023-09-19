using System;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf;

public class CachedOrderPdfService : IOrderPdfService
{
    private readonly IOrderPdfService orderPdfService;
    private readonly IAzureBlobStorageService azureBlobStorageService;
    private readonly AzureBlobSettings settings;

    public CachedOrderPdfService(
        IOrderPdfService orderPdfService,
        IAzureBlobStorageService azureBlobStorageService,
        AzureBlobSettings settings)
    {
        this.orderPdfService = orderPdfService ?? throw new ArgumentNullException(nameof(orderPdfService));
        this.azureBlobStorageService =
            azureBlobStorageService ?? throw new ArgumentNullException(nameof(azureBlobStorageService));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<MemoryStream> CreateOrderSummaryPdf(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        if (order.OrderStatus != OrderStatus.Completed && order.OrderStatus != OrderStatus.Terminated)
            return await orderPdfService.CreateOrderSummaryPdf(order);

        var blobDocument = order.OrderStatus == OrderStatus.Completed ? $"{order.CallOffId.ToString()}.pdf" : $"{order.CallOffId.ToString()}-terminated.pdf";
        var cachedPdf = await azureBlobStorageService.DownloadAsync(new(settings.OrderPdfContainerName, blobDocument));
        if (cachedPdf != null)
            return cachedPdf;

        var file = await orderPdfService.CreateOrderSummaryPdf(order);

        await azureBlobStorageService.UploadAsync(new(settings.OrderPdfContainerName, blobDocument), file);

        return file;
    }
}
