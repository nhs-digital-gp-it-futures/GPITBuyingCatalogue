using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ValidationSettingsTests
    {
        [Test]
        public static void ValidationSettings_MaxDeliveryDateOffsetInDays_CorrectlyReturned()
        {
            var settings = new ValidationSettings();

            settings.MaxDeliveryDateWeekOffset = 10;

            Assert.AreEqual(70, settings.MaxDeliveryDateOffsetInDays);
        }
    }
}
