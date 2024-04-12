using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyingCatalogueFunction.Notifications.Interfaces
{
    public interface IGovNotifyEmailService
    {
        Task<string[]> CheckForExisting(string notificationId);

        Task<string> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation, string notficationId);
    }
}
