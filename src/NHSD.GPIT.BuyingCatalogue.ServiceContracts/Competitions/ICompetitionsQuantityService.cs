using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsQuantityService
{
    Task SetSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients);

    Task SetServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients);

    Task SetAdditionalServiceAssociatedServiceQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceId,
        CatalogueItemId serviceId,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients);

    Task ResetSolutionQuantities(string internalOrgId, int competitionId, CatalogueItemId solutionId);

    Task ResetServiceQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId);

    Task ResetAdditionalServiceAssociatedServiceQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceId,
        CatalogueItemId serviceId);
}
