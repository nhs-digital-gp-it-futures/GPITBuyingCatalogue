using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SmtpSettingsTests
    {
        [Test]
        public static void SmtpSettings_AuthenticationSettings_IsInitialized()
        {
            var settings = new SmtpSettings();

            settings.Authentication.Should().NotBeNull();
        }
    }
}
