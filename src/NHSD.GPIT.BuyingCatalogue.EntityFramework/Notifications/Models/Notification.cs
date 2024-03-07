using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class Notification
    {
        public Guid Id { get; set; }

        public string To { get; set; }

        public NotificationTypeEnum NotificationType { get; set; }

        public string Json { get; set; }

        public string ReceiptId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public T JsonAs<T>()
            where T : GovNotifyEmailContent
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Json);
        }

        public void JsonFrom<T>(T obj)
            where T : GovNotifyEmailContent
        {
            NotificationType = obj.NotificationType;
            Json = System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
