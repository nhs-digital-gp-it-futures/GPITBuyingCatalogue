using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionNonPriceElementsService
{
    Task DeleteNonPriceElement(string internalOrgId, int competitionId, NonPriceElement nonPriceElement);

    Task AddFeatureRequirement(
        string internalOrgId,
        int competitionId,
        string requirements,
        CompliancyLevel compliance);

    Task EditFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string requirements,
        CompliancyLevel compliance);
}
