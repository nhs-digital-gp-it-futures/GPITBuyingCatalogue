using System;
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
    }
}
