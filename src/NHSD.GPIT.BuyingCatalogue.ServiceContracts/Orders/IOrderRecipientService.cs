using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

public interface IOrderRecipientService
{
    Task SetOrderRecipients(string internalOrgId, CallOffId callOffId, IEnumerable<string> odsCodes);
}
