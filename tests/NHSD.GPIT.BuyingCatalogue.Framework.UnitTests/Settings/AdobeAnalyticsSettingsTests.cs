using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    public static class AdobeAnalyticsSettingsTests
    {
        [Theory]
        [AutoData]
        public static void ScriptSource_EqualsExpectedValue(AdobeAnalyticsSettings settings)
        {
            var expected = new Uri(settings.BaseUrl, settings.FileName);

            settings.ScriptSource.Should().Be(expected);
        }
    }
}
