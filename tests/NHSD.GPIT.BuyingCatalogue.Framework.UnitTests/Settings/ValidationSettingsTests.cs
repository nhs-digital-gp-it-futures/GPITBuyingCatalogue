using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    public static class ValidationSettingsTests
    {
        [Fact]
        public static void ValidationSettings_MaxDeliveryDateOffsetInDays_CorrectlyReturned()
        {
            var settings = new ValidationSettings { MaxDeliveryDateWeekOffset = 10 };

            Assert.Equal(70, settings.MaxDeliveryDateOffsetInDays);
        }
    }
}
