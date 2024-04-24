namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public enum PasswordExpiryEventTypeEnum
{
    Nothing = EventTypeEnum.Nothing,
    PasswordEnteredFirstExpiryThreshold = EventTypeEnum.PasswordEnteredFirstExpiryThreshold,
    PasswordEnteredSecondExpiryThreshold = EventTypeEnum.PasswordEnteredSecondExpiryThreshold,
    PasswordEnteredThirdExpiryThreshold = EventTypeEnum.PasswordEnteredThirdExpiryThreshold,
}
