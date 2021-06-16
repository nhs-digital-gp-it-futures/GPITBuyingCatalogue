using System;
using FluentAssertions;
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
    }
}
