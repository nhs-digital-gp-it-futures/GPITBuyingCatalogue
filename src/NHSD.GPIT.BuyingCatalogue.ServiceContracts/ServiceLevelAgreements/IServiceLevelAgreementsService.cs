using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements
{
    public interface IServiceLevelAgreementsService
    {
        Task<EntityFramework.Catalogue.Models.ServiceLevelAgreements> GetAllServiceLevelAgreementsForSolution(CatalogueItemId solutionId);

        Task AddServiceLevelAsync(AddSlaModel model);

        Task UpdateServiceLevelTypeAsync(CatalogueItem solution, SlaType slaLevel);
    }
}
