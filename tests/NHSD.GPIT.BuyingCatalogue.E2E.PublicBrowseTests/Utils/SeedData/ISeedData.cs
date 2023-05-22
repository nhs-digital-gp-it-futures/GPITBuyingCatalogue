using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal interface ISeedData
{
    static abstract Task Initialize(BuyingCatalogueDbContext context);
}
