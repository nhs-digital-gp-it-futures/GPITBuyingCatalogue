using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions
{
    public class CompetitionSublocationService(BuyingCatalogueDbContext dbContext) : ICompetitionSublocationService
    {
        private readonly BuyingCatalogueDbContext dbContext =
            dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<CompetitionSublocation> GetCompetitionSublocationWithRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationId)
        {
            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .AsNoTracking()
                .Where(
                    x => x.OwnerOdsCode == externalOrgId
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationId)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .ThenInclude(y => y.RecipientOrganisation)
                .FirstOrDefaultAsync();
            return sublocation;
        }

        public Task AddSublocationRecipients(
            string externalOrgId,
            int competitionId,
            HashSet<string> sublocationIds)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSublocationRecipients(
            string externalOrgId,
            int competitionId,
            HashSet<string> sublocationIds)
        {
            throw new NotImplementedException();
        }
    }
}
