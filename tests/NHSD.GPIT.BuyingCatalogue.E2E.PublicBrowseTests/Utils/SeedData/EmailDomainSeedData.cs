using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal static class EmailDomainSeedData
{
    internal static void Initialize(BuyingCatalogueDbContext context)
    {
        context.EmailDomains.Add(new("@nhs.net"));
        context.EmailDomains.Add(new("@email.com"));

        context.SaveChanges();
    }
}
