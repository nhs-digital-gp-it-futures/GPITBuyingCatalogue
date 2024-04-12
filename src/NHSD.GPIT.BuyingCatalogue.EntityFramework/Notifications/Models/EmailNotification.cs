using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class EmailNotification
    {
        public Guid Id { get; set; }

        public string To { get; set; }

        public EmailNotificationTypeEnum EmailNotificationType { get; set; }

        public string Json { get; set; }

        public string ReceiptId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public T JsonAs<T>()
            where T : EmailModel
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Json);
        }

        public void JsonFrom<T>(T obj)
            where T : EmailModel
        {
            EmailNotificationType = obj.NotificationType;
            Json = System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
