using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans
{
    public interface IDevelopmentPlansService
    {
        Task SaveDevelopmentPlans(CatalogueItemId solutionId, string developmentPlan);

        Task<List<WorkOffPlan>> GetWorkOffPlans(CatalogueItemId solutionId);

        Task<WorkOffPlan> GetWorkOffPlan(int id);

        Task SaveWorkOffPlan(CatalogueItemId solutionId, SaveWorkOffPlanModel model);

        Task UpdateWorkOffPlan(int id, SaveWorkOffPlanModel model);

        Task DeleteWorkOffPlan(int id);
    }
}
