using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemRecipientService
    {
        Task AddOrderItemRecipients(int orderId, CatalogueItemId catalogueItemId, IEnumerable<ServiceRecipientDto> recipients);
    }
}
