namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public enum EventTypeEnum
    {
        OrderCompleted = 1,
        OrderEnteredFirstExpiryThreshold = 2,
        OrderEnteredSecondExpiryThreshold = 3,
        UserPasswordExpired = 4,
    }
}
