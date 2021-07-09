using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class ClientApplicationTypesModelTests
    {
        [Fact]
        public static void IsComplete_BrowserBasedIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel { BrowserBased = true, };

            model.IsComplete.Should().BeTrue();
        }

        [Fact]
        public static void IsComplete_NativeDesktopIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel { NativeDesktop = true, };

            model.IsComplete.Should().BeTrue();
        }

        [Fact]
        public static void IsComplete_NativeMobileIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel { NativeMobile = true, };

            model.IsComplete.Should().BeTrue();
        }

        [Fact]
        public static void IsComplete_AllBooleanPropertiesFalse_ReturnsFalse()
        {
            var model = new ClientApplicationTypesModel();

            model.IsComplete.Should().BeFalse();
        }
    }
}
