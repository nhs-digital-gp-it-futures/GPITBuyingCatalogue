using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements
{
    public interface IServiceLevelAgreementsService
    {
        Task<EntityFramework.Catalogue.Models.ServiceLevelAgreements> GetServiceLevelAgreementForSolution(CatalogueItemId solutionId);

        Task<ServiceAvailabilityTimes> GetServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId);

        Task AddServiceLevelAgreement(AddSlaModel model);

        Task UpdateServiceLevelTypeAsync(CatalogueItem solution, SlaType slaLevel);

        Task SaveServiceAvailabilityTimes(CatalogueItem solution, ServiceAvailabilityTimesModel model);

        Task UpdateServiceAvailabilityTimes(CatalogueItem solution, int serviceAvailabilityTimesId, ServiceAvailabilityTimesModel model);

        Task DeleteServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId);

        Task<int> GetCountOfServiceAvailabilityTimes(CatalogueItemId solutionId, params int[] idsToExclude);

        Task AddSLAContact(CatalogueItem solution, EditSLAContactModel model);

        Task EditSlaContact(EditSLAContactModel model);

        Task DeleteSlaContact(int slaContactId);

        Task AddServiceLevel(CatalogueItemId solutionId, EditServiceLevelModel model);

        Task UpdateServiceLevel(CatalogueItemId solutionId, int serviceLevelId, EditServiceLevelModel model);

        Task DeleteServiceLevel(CatalogueItemId solutionId, int serviceLevelId);
    }
}
