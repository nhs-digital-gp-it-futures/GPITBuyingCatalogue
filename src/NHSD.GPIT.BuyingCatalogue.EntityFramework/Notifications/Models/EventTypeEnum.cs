namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public enum EventTypeEnum
    {
        Nothing = 0,
        OrderEnteredFirstExpiryThreshold = 1,
        OrderEnteredSecondExpiryThreshold = 2,
        PasswordEnteredFirstExpiryThreshold = 3,
        PasswordEnteredSecondExpiryThreshold = 4,
        PasswordEnteredThirdExpiryThreshold = 5,
        InactivityEnteredFirstExpiryThreshold = 6,
        InactivityEnteredSecondExpiryThreshold = 7,
        InactivityEnteredThirdExpiryThreshold = 8,
        InactivityEnteredForthExpiryThreshold = 9,
        InactivityEnteredFifthExpiryThreshold = 10,
    }
}
