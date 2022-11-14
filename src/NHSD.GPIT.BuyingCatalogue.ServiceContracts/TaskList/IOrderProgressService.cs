using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface IOrderProgressService
    {
        Task<OrderProgress> GetOrderProgress(string internalOrgId, CallOffId callOffId);
    }
}
