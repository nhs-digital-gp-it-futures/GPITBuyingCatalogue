using Microsoft.Extensions.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public class QueueOptions
    {
        public string SendEmailNotifications { get; set; } = string.Empty;

        public string CompleteEmailNotifications { get; set; } = string.Empty;
    }
}
