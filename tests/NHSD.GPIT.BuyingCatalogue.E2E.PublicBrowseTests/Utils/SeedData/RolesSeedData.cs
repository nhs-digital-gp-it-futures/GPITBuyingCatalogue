using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

public class RolesSeedData
{
    internal static void Initialize(BuyingCatalogueDbContext context)
    {
        context.Roles.Add(new() { Name = OrganisationFunction.AuthorityName, NormalizedName = OrganisationFunction.AuthorityName.ToUpperInvariant() });
        context.Roles.Add(new() { Name = OrganisationFunction.BuyerName, NormalizedName = OrganisationFunction.BuyerName.ToUpperInvariant() });

        context.SaveChanges();
    }
}
