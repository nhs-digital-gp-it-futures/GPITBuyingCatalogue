using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

public class FiltersSeedData : ISeedData
{
    public static async Task Initialize(BuyingCatalogueDbContext context)
    {
        var capability = await context.Capabilities.FirstOrDefaultAsync(x => x.Id == 43);
        var filters = new List<Filter>()
        {
            new()
            {
                Id = 1,
                Name = "Filter 1",
                Description = "GPIT Framework Filter",
                OrganisationId = 77,
                FrameworkId = "NHSDGP001",
                FilterCapabilityEpics =
                    new List<FilterCapabilityEpic> { new() { CapabilityId = capability.Id } },
                FilterHostingTypes = new List<FilterHostingType>
                {
                    new() { FilterId = 1, HostingType = HostingType.Hybrid, },
                },
            },
        };

        await context.InsertRangeWithIdentityAsync(filters);
    }
}
