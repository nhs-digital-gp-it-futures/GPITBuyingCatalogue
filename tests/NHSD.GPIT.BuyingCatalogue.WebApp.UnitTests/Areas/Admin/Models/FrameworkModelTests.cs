using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class FrameworkModelTests
    {
        [Fact]
        public static void IsValid_NoFrameworkSelected_ReturnsFalse()
        {
            var frameworkModel = new FrameworkModel();

            frameworkModel.IsValid().Should().BeFalse();
        }

        [Fact]
        public static void IsValid_DfocvcFrameworkSelected_ReturnsTrue()
        {
            var frameworkModel = new FrameworkModel { DfocvcFramework = true };

            frameworkModel.IsValid().Should().BeTrue();
        }

        [Fact]
        public static void IsValid_GpitFuturesFrameworkSelected_ReturnsTrue()
        {
            var frameworkModel = new FrameworkModel { GpitFuturesFramework = true };

            frameworkModel.IsValid().Should().BeTrue();
        }
    }
}
