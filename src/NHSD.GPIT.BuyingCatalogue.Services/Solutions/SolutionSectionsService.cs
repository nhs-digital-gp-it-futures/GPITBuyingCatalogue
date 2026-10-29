using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions;

public class SolutionSectionsService(BuyingCatalogueDbContext context) : ISolutionSectionsService
{
    public async Task<SolutionSections> GetSolutionSectionsAsync(SolutionType solutionTypeId)
    {
        return await context.SolutionSections
            .Where(x => x.SolutionType == solutionTypeId)
            .FirstOrDefaultAsync();
    }
}
