using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

public interface ISolutionStandardsService
{
    Task<IEnumerable<StandardComplianceModel>> GetSolutionStandards(CatalogueItemId solutionId);

    Task<StandardComplianceModel> GetSolutionStandard(CatalogueItemId solutionId, string standardId);

    Task<IEnumerable<Standard>> GetInProgressStandards(CatalogueItemId solutionId);

    Task SetSolutionStandardStatus(CatalogueItemId solutionId, string standardId, StandardCompliance compliance);
}
