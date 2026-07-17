namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public enum InactiveAccountEventType
{
    Nothing = EventTypeEnum.Nothing,
    InactivityEnteredFirstExpiryThreshold = EventTypeEnum.InactivityEnteredFirstExpiryThreshold,
    InactivityEnteredSecondExpiryThreshold = EventTypeEnum.InactivityEnteredSecondExpiryThreshold,
    InactivityEnteredThirdExpiryThreshold = EventTypeEnum.InactivityEnteredThirdExpiryThreshold,
    InactivityEnteredForthExpiryThreshold = EventTypeEnum.InactivityEnteredForthExpiryThreshold,
    InactivityEnteredFifthExpiryThreshold = EventTypeEnum.InactivityEnteredFifthExpiryThreshold,
}
