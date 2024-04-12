using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public abstract class GovNotifyEmailModel(EmailNotificationTypeEnum notificationType)
    {
        public EmailNotificationTypeEnum NotificationType { get; set; } = notificationType;

        public abstract Dictionary<string, dynamic> GetTemplatePersonalisation();

        public abstract string GetTemplateId(TemplateOptions options);
    }
}
