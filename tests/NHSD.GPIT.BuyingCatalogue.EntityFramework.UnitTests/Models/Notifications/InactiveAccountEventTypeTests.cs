using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Notifications
{
    public static class InactiveAccountEventTypeTests
    {
        [Theory]
        [InlineAutoData(InactiveAccountEventType.Nothing, EventTypeEnum.Nothing)]
        [InlineAutoData(InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold, EventTypeEnum.InactivityEnteredFirstExpiryThreshold)]
        [InlineAutoData(InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold, EventTypeEnum.InactivityEnteredSecondExpiryThreshold)]
        [InlineAutoData(InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold, EventTypeEnum.InactivityEnteredThirdExpiryThreshold)]
        [InlineAutoData(InactiveAccountEventType.InactivityEnteredForthExpiryThreshold, EventTypeEnum.InactivityEnteredForthExpiryThreshold)]
        [InlineAutoData(InactiveAccountEventType.InactivityEnteredFifthExpiryThreshold, EventTypeEnum.InactivityEnteredFifthExpiryThreshold)]
        public static void InactiveAccountEventTypeEnum_Values(
        InactiveAccountEventType inactiveAccountEventTypeEnum,
        EventTypeEnum expectedEventTypeEnum)
        {
            inactiveAccountEventTypeEnum.Should().Be((InactiveAccountEventType)expectedEventTypeEnum);
        }
    }
}
