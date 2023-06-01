using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionsService : ICompetitionsService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionsService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Competition>> GetCompetitions(int organisationId)
        => await dbContext.Competitions.Where(x => x.OrganisationId == organisationId)
            .ToListAsync();

    public async Task AddCompetition(int organisationId, int filterId, string name, string description)
    {
        var competition = new Competition
        {
            OrganisationId = organisationId, FilterId = filterId, Name = name, Description = description,
        };

        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int organisationId, string competitionName) =>
        await dbContext.Competitions.AnyAsync(x => x.OrganisationId == organisationId && string.Equals(x.Name, competitionName));
}
