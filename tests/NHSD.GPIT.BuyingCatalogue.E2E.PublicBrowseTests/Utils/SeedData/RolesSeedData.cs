using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

public class RolesSeedData : ISeedData
{
    public static async Task Initialize(BuyingCatalogueDbContext context)
    {
        context.Roles.Add(new() { Name = OrganisationFunction.Authority.Name, NormalizedName = OrganisationFunction.Authority.Name.ToUpperInvariant() });
        context.Roles.Add(new() { Name = OrganisationFunction.Buyer.Name, NormalizedName = OrganisationFunction.Buyer.Name.ToUpperInvariant() });
        context.Roles.Add(new() { Name = OrganisationFunction.AccountManager.Name, NormalizedName = OrganisationFunction.AccountManager.Name.ToUpperInvariant() });
        await context.SaveChangesAsync();
    }
}
