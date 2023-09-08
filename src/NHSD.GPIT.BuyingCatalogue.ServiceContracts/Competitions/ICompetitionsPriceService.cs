using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsPriceService
{
    Task SetSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices);

    Task SetServicePrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices);
}
