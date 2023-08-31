using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Pdf;

public static class CachedOrderPdfServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CachedOrderPdfService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_IncompleteOrder_DoesNotCachePdf(
        Order order,
        byte[] fileContents,
        [Frozen] Mock<IOrderPdfService> orderPdfService,
        [Frozen] Mock<IAzureBlobStorageService> azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = null;

        orderPdfService.Setup(x => x.CreateOrderSummaryPdf(It.IsAny<Order>()))
            .ReturnsAsync(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        orderPdfService.VerifyAll();
        azureBlobStorageService.VerifyNoOtherCalls();

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_CompleteOrder_CachesPdf(
        Order order,
        byte[] fileContents,
        [Frozen] Mock<IOrderPdfService> orderPdfService,
        [Frozen] Mock<IAzureBlobStorageService> azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = DateTime.UtcNow;

        orderPdfService.Setup(x => x.CreateOrderSummaryPdf(It.IsAny<Order>()))
            .ReturnsAsync(new MemoryStream(fileContents));

        azureBlobStorageService
            .Setup(x => x.DownloadAsync(It.IsAny<BlobDocument>()))
            .ReturnsAsync((MemoryStream)null);

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        orderPdfService.VerifyAll();
        azureBlobStorageService.VerifyAll();
        azureBlobStorageService.Verify(x => x.UploadAsync(It.IsAny<BlobDocument>(), It.IsAny<Stream>()));

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_CachedCompletedOrder_DownloadsCachedPdf(
        Order order,
        byte[] fileContents,
        [Frozen] Mock<IAzureBlobStorageService> azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = DateTime.UtcNow;

        azureBlobStorageService
            .Setup(x => x.DownloadAsync(It.IsAny<BlobDocument>()))
            .ReturnsAsync(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_TerminatedOrder_CachesPdf(
        Order order,
        byte[] fileContents,
        [Frozen] Mock<IOrderPdfService> orderPdfService,
        [Frozen] Mock<IAzureBlobStorageService> azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.IsTerminated = true;

        orderPdfService.Setup(x => x.CreateOrderSummaryPdf(It.IsAny<Order>()))
            .ReturnsAsync(new MemoryStream(fileContents));

        azureBlobStorageService
            .Setup(x => x.DownloadAsync(It.IsAny<BlobDocument>()))
            .ReturnsAsync((MemoryStream)null);

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        orderPdfService.VerifyAll();
        azureBlobStorageService.VerifyAll();
        azureBlobStorageService.Verify(x => x.UploadAsync(It.IsAny<BlobDocument>(), It.IsAny<Stream>()));

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_CachedTerminatedOrder_DownloadsCachedPdf(
        Order order,
        byte[] fileContents,
        [Frozen] Mock<IAzureBlobStorageService> azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.IsTerminated = true;

        azureBlobStorageService
            .Setup(x => x.DownloadAsync(It.IsAny<BlobDocument>()))
            .ReturnsAsync(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }
}
