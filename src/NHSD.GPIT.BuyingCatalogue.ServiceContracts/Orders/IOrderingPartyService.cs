using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderingPartyService
    {
        Task SetOrderingPartyContact(CallOffId callOffId, Contact contact);
    }
}
