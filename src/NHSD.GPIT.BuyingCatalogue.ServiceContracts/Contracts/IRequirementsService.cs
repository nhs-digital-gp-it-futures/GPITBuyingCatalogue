using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IRequirementsService
    {
        Task<Contract> SetRequirementComplete(int orderId, int contractId);

        Task AddRequirement(
            int orderId,
            int contractId,
            CatalogueItemId catalogueItemId,
            string details);

        Task<Requirement> GetRequirement(int orderId, int requirementId);

        Task EditRequirement(
            int orderId,
            int requirementId,
            CatalogueItemId catalogueItemId,
            string details);

        Task DeleteRequirement(int orderId, int requirementId);

        Task DeleteRequirements(int orderId, IEnumerable<CatalogueItemId> catalogueItemIds);
    }
}
