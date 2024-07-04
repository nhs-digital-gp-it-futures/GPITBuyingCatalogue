using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public abstract class GovNotifyEmailModel(EmailNotificationTypeEnum notificationType)
    {
        public EmailNotificationTypeEnum NotificationType { get; } = notificationType;

        public abstract Dictionary<string, dynamic> GetTemplatePersonalisation();

        public abstract string GetTemplateId(TemplateOptions options);
    }
}
