using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IAssociatedServicesBillingService
    {
        Task<List<OrderItem>> GetAssociatedServiceOrderItems(string internalOrgId, CallOffId callOffId);
    }
}
