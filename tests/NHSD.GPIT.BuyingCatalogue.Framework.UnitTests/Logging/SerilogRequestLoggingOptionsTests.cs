using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NUnit.Framework;
using Serilog.Events;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Logging
{
    [TestFixture]
    internal sealed class SerilogRequestLoggingOptionsTests
    {
        [Test]
        public void GetLevel_Exception_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, new InvalidOperationException())
                .Should().Be(LogEventLevel.Error);
        }

        [Test]
        public void GetLevel_NullHttpContext_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, null)
                .Should().Be(LogEventLevel.Error);
        }

        [Test]
        public void GetLevel_HttpContext_NonErrorCode_ReturnsInformation()
        {
            var httpResponse = new Mock<HttpResponse>();

            httpResponse.Setup(x => x.StatusCode).Returns(498);

            var httpContext = new Mock<HttpContext>();

            httpContext.Setup(x => x.Response).Returns(httpResponse.Object);

            SerilogRequestLoggingOptions.GetLevel(httpContext.Object, 0, null)
                .Should().Be(LogEventLevel.Information);
        }

        [Test]
        public void GetLevel_HttpContext_ErrorCode_ReturnsError()
        {
            var httpResponse = new Mock<HttpResponse>();

            httpResponse.Setup(x => x.StatusCode).Returns(500);

            var httpContext = new Mock<HttpContext>();

            httpContext.Setup(x => x.Response).Returns(httpResponse.Object);

            SerilogRequestLoggingOptions.GetLevel(httpContext.Object, 0, null)
                .Should().Be(LogEventLevel.Error);
        }
    }
}
