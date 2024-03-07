using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public abstract class GovNotifyEmailContent(NotificationTypeEnum notificationType)
    {
        public NotificationTypeEnum NotificationType { get; set; } = notificationType;

        public abstract Dictionary<string, dynamic> GetTemplatePersonalisation(Func<byte[], bool, JObject> prepareUpload);

        public abstract string GetTemplateId();
    }
}
