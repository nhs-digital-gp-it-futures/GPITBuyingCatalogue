using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Pdf;

public static class OrderPdfServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(OrderPdfService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task CreateOrderSummaryPdf_CallsPdfService(
        Order order,
        byte[] fileContents,
        ActionContext actionContext,
        [Frozen] Mock<IPdfService> pdfService,
        [Frozen] Mock<IActionContextAccessor> actionContextAccessor,
        OrderPdfService orderPdfService)
    {
        actionContextAccessor
            .SetupGet(x => x.ActionContext)
            .Returns(actionContext);

        pdfService
            .Setup(x => x.Convert(It.IsAny<Uri>()))
            .ReturnsAsync(fileContents);

        pdfService
            .Setup(x => x.BaseUri())
            .Returns(new Uri("https://localhost"));

        var pdfContents = await orderPdfService.CreateOrderSummaryPdf(order);

        pdfService.VerifyAll();

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }
}
