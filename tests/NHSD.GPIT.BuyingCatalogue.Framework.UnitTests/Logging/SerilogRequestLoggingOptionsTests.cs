using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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

        [Fact]
        public static void GetLevel_HttpContext_NonErrorCode_ReturnsInformation()
        {
            var httpResponse = new Mock<HttpResponse>();

            httpResponse.Setup(r => r.StatusCode).Returns(498);

            var httpContext = new Mock<HttpContext>();

            httpContext.Setup(c => c.Response).Returns(httpResponse.Object);

            SerilogRequestLoggingOptions.GetLevel(httpContext.Object, 0, null)
                .Should().Be(LogEventLevel.Information);
        }

        [Fact]
        public static void GetLevel_HttpContext_ErrorCode_ReturnsError()
        {
            var httpResponse = new Mock<HttpResponse>();

            httpResponse.Setup(r => r.StatusCode).Returns(500);

            var httpContext = new Mock<HttpContext>();

            httpContext.Setup(c => c.Response).Returns(httpResponse.Object);

            SerilogRequestLoggingOptions.GetLevel(httpContext.Object, 0, null)
                .Should().Be(LogEventLevel.Error);
        }
    }
}
