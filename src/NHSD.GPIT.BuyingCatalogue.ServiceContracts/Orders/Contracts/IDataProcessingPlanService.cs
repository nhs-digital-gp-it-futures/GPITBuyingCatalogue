using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders.Contracts;

public interface IDataProcessingPlanService
{
    Task<DataProcessingPlan> CreateDataProcessingPlan();

    Task<DataProcessingPlan> GetDefaultDataProcessingPlan();
}
