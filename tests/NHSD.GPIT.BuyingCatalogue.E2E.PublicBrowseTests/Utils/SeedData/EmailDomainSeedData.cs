using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal class EmailDomainSeedData : ISeedData
{
    public static async Task Initialize(BuyingCatalogueDbContext context)
    {
        context.EmailDomains.Add(new("@nhs.net"));
        context.EmailDomains.Add(new("@email.com"));

        await context.SaveChangesAsync();
    }
}
