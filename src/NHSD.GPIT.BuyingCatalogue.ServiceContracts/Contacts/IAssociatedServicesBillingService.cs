using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts
{
    public interface IAssociatedServicesBillingService
    {
        Task<List<OrderItem>> GetAssociatedServiceOrderItems(string internalOrgId, CallOffId callOffId);
    }
}
