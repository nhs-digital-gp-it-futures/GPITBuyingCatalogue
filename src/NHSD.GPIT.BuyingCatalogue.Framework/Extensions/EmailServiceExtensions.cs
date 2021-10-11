using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class EmailServiceExtensions
    {
        public static async Task SendEmailAsync(
            this IEmailService service,
            EmailMessageTemplate messageTemplate,
            EmailAddress recipient,
            params object[] formatItems)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            await service.SendEmailAsync(new EmailMessage(messageTemplate, new[] { recipient }, null, formatItems));
        }
    }
}
