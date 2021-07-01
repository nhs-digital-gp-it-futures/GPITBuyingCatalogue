using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    public static class SmtpSettingsTests
    {
        [Fact]
        public static void SmtpSettings_AuthenticationSettings_IsInitialized()
        {
            var settings = new SmtpSettings();

            settings.Authentication.Should().NotBeNull();
        }
    }
}
