using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    [ExcludeFromCodeCoverage]
    public class QueueOptions
    {
        public string SendEmailNotifications { get; set; } = string.Empty;

        public string CompleteEmailNotifications { get; set; } = string.Empty;
    }
}
