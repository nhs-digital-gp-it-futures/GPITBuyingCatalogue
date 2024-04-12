using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace BuyingCatalogueFunction.Notifications.Templates
{
    public abstract class GovNotifyEmailTemplate(EmailNotificationTypeEnum notificationType)
    {
        public EmailNotificationTypeEnum NotificationType { get; set; } = notificationType;

        public abstract Dictionary<string, dynamic> GetTemplatePersonalisation();

        public abstract string GetTemplateId(TemplateOptions options);
    }
}
