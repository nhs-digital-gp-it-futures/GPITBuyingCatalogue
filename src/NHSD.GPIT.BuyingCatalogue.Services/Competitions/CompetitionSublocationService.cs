using System;
using System.Threading.Tasks;
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
            string internalOrgId,
            int competitionId)
        {
            throw new NotImplementedException();
        }
    }
}
