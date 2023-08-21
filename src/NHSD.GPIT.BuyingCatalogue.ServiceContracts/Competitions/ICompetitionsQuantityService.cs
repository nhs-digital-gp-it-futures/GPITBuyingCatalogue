using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsQuantityService
{
    Task SetSolutionGlobalQuantity(string internalOrgId, int competitionId, CatalogueItemId solutionId, int quantity);

    Task SetServiceGlobalQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        int quantity);

    Task SetSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<ServiceRecipientDto> serviceRecipients);

    Task SetServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        IEnumerable<ServiceRecipientDto> serviceRecipients);

    Task ResetSolutionQuantities(string internalOrgId, int competitionId, CatalogueItemId solutionId);

    Task ResetServiceQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId);
}
