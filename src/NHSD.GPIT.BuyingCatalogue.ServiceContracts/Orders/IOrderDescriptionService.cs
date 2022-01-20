using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderDescriptionService
    {
        Task SetOrderDescription(CallOffId callOffId, string odsCode, string description);
    }
}
