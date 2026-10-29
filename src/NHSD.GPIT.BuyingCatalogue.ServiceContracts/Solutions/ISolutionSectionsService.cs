using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

public interface ISolutionSectionsService
{
    Task<SolutionSections> GetSolutionSectionsAsync(SolutionType solutionTypeId);
}
