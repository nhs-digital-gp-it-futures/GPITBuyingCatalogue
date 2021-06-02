using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderingPartyService
    {
        Task SetOrderingParty(Order order, OrderingParty orderingParty, Contact contact);
    }
}
