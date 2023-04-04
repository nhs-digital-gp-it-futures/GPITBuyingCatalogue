using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services;

public class StubbedGpPracticeProvider : IGpPracticeProvider
{
    private static Random random = new Random();
    private readonly BuyingCatalogueDbContext dbContext;

    public StubbedGpPracticeProvider(BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<GpPractice>> GetGpPractices(Uri csvUri)
    {
        var subLocations = dbContext.OrganisationRelationships.Where(x => x.OwnerOrganisationId == "03F")
            .Select(x => x.TargetOrganisationId);
        var serviceRecipients = await dbContext.OrganisationRelationships
            .Where(x => subLocations.Contains(x.OwnerOrganisationId))
            .Select(x => x.TargetOrganisation)
            .Take(3)
            .Select(
                x => new GpPractice
                {
                    CODE = x.Id, EXTRACT_DATE = DateTime.UtcNow, NUMBER_OF_PATIENTS = random.Next(int.MaxValue),
                })
            .ToListAsync();

        return serviceRecipients;
    }
}
