using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Logging
{
    public static class SerilogRequestLoggingOptionsTests
    {
        [Fact]
        public static void GetLevel_Exception_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, new InvalidOperationException())
                .Should().Be(LogEventLevel.Error);
        }

        [Fact]
        public static void GetLevel_NullHttpContext_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, null)
                .Should().Be(LogEventLevel.Error);
        }

        [Theory]
        [MockAutoData]
        public static void GetLevel_HttpContext_NonErrorCode_ReturnsInformation(
            [Frozen] HttpResponse httpResponse,
            [Frozen] HttpContext httpContext)
        {
            httpResponse.StatusCode.Returns(498);
            httpContext.Response.Returns(httpResponse);

            SerilogRequestLoggingOptions.GetLevel(httpContext, 0, null)
                .Should().Be(LogEventLevel.Information);
        }

        [Theory]
        [MockAutoData]
        public static void GetLevel_HttpContext_ErrorCode_ReturnsError(
            [Frozen] HttpResponse httpResponse,
            [Frozen] HttpContext httpContext)
        {
            httpResponse.StatusCode.Returns(500);
            httpContext.Response.Returns(httpResponse);

            SerilogRequestLoggingOptions.GetLevel(httpContext, 0, null)
                .Should().Be(LogEventLevel.Error);
        }
    }
}
