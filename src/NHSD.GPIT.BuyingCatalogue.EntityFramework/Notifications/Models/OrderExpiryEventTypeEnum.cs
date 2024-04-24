namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

public enum OrderExpiryEventTypeEnum
{
    Nothing = EventTypeEnum.Nothing,
    OrderEnteredFirstExpiryThreshold = EventTypeEnum.OrderEnteredFirstExpiryThreshold,
    OrderEnteredSecondExpiryThreshold = EventTypeEnum.OrderEnteredSecondExpiryThreshold,
}
