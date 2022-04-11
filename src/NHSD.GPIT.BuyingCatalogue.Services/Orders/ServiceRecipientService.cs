using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class ServiceRecipientService : IServiceRecipientService
    {
        private readonly BuyingCatalogueDbContext context;

        public ServiceRecipientService(BuyingCatalogueDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddServiceRecipient(ServiceRecipientDto recipient)
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            var serviceRecipient = context.ServiceRecipients
                .FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

            if (serviceRecipient == null)
            {
                serviceRecipient = new ServiceRecipient
                {
                    OdsCode = recipient.OdsCode,
                };

                context.ServiceRecipients.Add(serviceRecipient);
            }

            if (serviceRecipient.Name != recipient.Name)
            {
                serviceRecipient.Name = recipient.Name;

                await context.SaveChangesAsync();
            }
        }
    }
}
