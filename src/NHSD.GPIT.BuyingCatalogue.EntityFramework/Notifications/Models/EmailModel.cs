namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public abstract class EmailModel(EmailNotificationTypeEnum notificationType)
    {
        public EmailNotificationTypeEnum NotificationType { get; set; } = notificationType;
    }
}
