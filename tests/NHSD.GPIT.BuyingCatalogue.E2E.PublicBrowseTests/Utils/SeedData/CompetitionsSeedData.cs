using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal class CompetitionsSeedData : ISeedData
{
    public static async Task Initialize(BuyingCatalogueDbContext context)
    {
        var user = await context.AspNetUsers.FirstAsync(x => x.Id == UserSeedData.SueId);

        var competitions = new List<Competition>
        {
            new()
            {
                Id = 1,
                FilterId = (await context.Filters.FirstAsync()).Id,
                Name = "Test Competition #1",
                Description = "Test Competition #1",
                LastUpdatedBy = user.Id,
                OrganisationId = user.PrimaryOrganisationId,
                LastUpdated = DateTime.UtcNow,
            },
        };

        context.InsertRangeWithIdentity(competitions);
    }
}
