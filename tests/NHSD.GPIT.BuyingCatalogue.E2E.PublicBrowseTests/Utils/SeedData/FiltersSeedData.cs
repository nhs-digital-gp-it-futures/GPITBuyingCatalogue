using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

public class FiltersSeedData : ISeedData
{
    public static async Task Initialize(BuyingCatalogueDbContext context)
    {
        List<Filter> filters = new()
        {
            new Filter
            {
                Id = 1,
                Name = "Filter 1",
                Description = "GPIT Framework Filter",
                OrganisationId = 77,
                FrameworkId = "NHSDGP001",
                FilterCapabilities =
                    new List<FilterCapability>() { new() { FilterId = 1, CapabilityId = 43, }, },
                FilterHostingTypes = new List<FilterHostingType>()
                {
                    new() { FilterId = 1, HostingType = HostingType.Hybrid, },
                },
            },
        };

        await context.InsertRangeWithIdentityAsync(filters);
    }
}
