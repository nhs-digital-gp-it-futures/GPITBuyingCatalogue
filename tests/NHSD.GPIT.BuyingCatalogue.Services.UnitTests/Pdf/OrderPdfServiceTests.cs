using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Pdf;

public static class OrderPdfServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(OrderPdfService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task CreateOrderSummaryPdf_CallsPdfService(
        Order order,
        byte[] fileContents,
        DefaultHttpContext httpContext,
        [Frozen] IPdfService pdfService,
        [Frozen] IUrlHelperFactory urlHelperFactory,
        [Frozen] IServiceProvider serviceProvider,
        [Frozen] IHttpContextAccessor httpContextAccessor,
        OrderPdfService orderPdfService)
    {
        serviceProvider.GetService(typeof(IUrlHelperFactory)).Returns(urlHelperFactory);
        httpContext.RequestServices = serviceProvider;

        httpContextAccessor.HttpContext.Returns(httpContext);

        pdfService.Convert(Arg.Any<Uri>()).Returns(fileContents);

        pdfService.BaseUri().Returns(new Uri("https://localhost"));

        var pdfContents = await orderPdfService.CreateOrderSummaryPdf(order);

        await pdfService.Received().Convert(Arg.Any<Uri>());
        pdfService.Received().BaseUri();

        pdfContents.ToArray().Should().BeEquivalentTo(fileContents);
    }
}
