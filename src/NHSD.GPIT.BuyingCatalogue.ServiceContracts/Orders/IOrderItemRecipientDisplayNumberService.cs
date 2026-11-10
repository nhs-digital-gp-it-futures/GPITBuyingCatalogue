using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

public interface IOrderItemRecipientDisplayNumberService
{
    Task SetOrderItemRecipientDisplayNumbers(CallOffId callOffId, string internalOrgId);

    Task<bool> HasItemRecipientCountDiscrepancy(CallOffId callOffId, string internalOrgId);
}
