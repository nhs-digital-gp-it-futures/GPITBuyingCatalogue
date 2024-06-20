using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Pdf;

public static class CachedOrderPdfServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CachedOrderPdfService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_IncompleteOrder_DoesNotCachePdf(
        Order order,
        byte[] fileContents,
        [Frozen] IOrderPdfService orderPdfService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = null;

        orderPdfService.CreateOrderSummaryPdf(Arg.Any<Order>()).Returns(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        await orderPdfService.Received().CreateOrderSummaryPdf(Arg.Any<Order>());

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_CompleteOrder_CachesPdf(
        Order order,
        byte[] fileContents,
        [Frozen] IOrderPdfService orderPdfService,
        [Frozen] IAzureBlobStorageService azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = DateTime.UtcNow;

        orderPdfService.CreateOrderSummaryPdf(Arg.Any<Order>()).Returns(new MemoryStream(fileContents));

        azureBlobStorageService.DownloadAsync(Arg.Any<BlobDocument>()).Returns((MemoryStream)null);

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        await orderPdfService.Received().CreateOrderSummaryPdf(Arg.Any<Order>());
        await azureBlobStorageService.Received().DownloadAsync(Arg.Any<BlobDocument>());
        await azureBlobStorageService.Received().UploadAsync(Arg.Any<BlobDocument>(), Arg.Any<Stream>());

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_CachedCompletedOrder_DownloadsCachedPdf(
        Order order,
        byte[] fileContents,
        [Frozen] IAzureBlobStorageService azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.Completed = DateTime.UtcNow;

        azureBlobStorageService.DownloadAsync(Arg.Any<BlobDocument>()).Returns(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_TerminatedOrder_CachesPdf(
        Order order,
        byte[] fileContents,
        [Frozen] IOrderPdfService orderPdfService,
        [Frozen] IAzureBlobStorageService azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.IsTerminated = true;

        orderPdfService.CreateOrderSummaryPdf(Arg.Any<Order>()).Returns(new MemoryStream(fileContents));

        azureBlobStorageService.DownloadAsync(Arg.Any<BlobDocument>()).Returns((MemoryStream)null);

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        await orderPdfService.Received().CreateOrderSummaryPdf(Arg.Any<Order>());
        await azureBlobStorageService.Received().DownloadAsync(Arg.Any<BlobDocument>());
        await azureBlobStorageService.Received().UploadAsync(Arg.Any<BlobDocument>(), Arg.Any<Stream>());

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_CachedTerminatedOrder_DownloadsCachedPdf(
        Order order,
        byte[] fileContents,
        [Frozen] IAzureBlobStorageService azureBlobStorageService,
        CachedOrderPdfService cachedOrderPdfService)
    {
        order.IsTerminated = true;

        azureBlobStorageService.DownloadAsync(Arg.Any<BlobDocument>()).Returns(new MemoryStream(fileContents));

        var pdfContents = await cachedOrderPdfService.CreateOrderSummaryPdf(order);

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }
}
