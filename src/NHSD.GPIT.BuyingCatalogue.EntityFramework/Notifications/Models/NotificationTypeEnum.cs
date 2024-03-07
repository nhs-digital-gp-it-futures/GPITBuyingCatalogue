namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public enum NotificationTypeEnum
    {
        BuyerOrderCompleted = 1,
        FinanceOrderCompleted = 2,
        OrderEnteredFirstExpiryThreshold = 3,
        OrderEnteredSecondExpiryThreshold = 4,
    }
}
