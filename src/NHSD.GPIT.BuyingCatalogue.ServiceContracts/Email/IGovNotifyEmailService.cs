using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    public interface IGovNotifyEmailService
    {
        Task SendEmailAsync(
            string emailAddress,
            string templateId,
            Dictionary<string, dynamic> personalisation);
    }
}
