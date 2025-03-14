using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions
{
    public class CompetitionSublocationService(BuyingCatalogueDbContext dbContext, IOdsService odsService)
        : ICompetitionSublocationService
    {
        private readonly BuyingCatalogueDbContext dbContext =
            dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

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
                .FirstAsync();
            return sublocation;
        }

        public async Task AddSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationId,
            HashSet<string> recipientIds)
        {
            ArgumentException.ThrowIfNullOrEmpty(externalOrgId, nameof(externalOrgId));
            ArgumentException.ThrowIfNullOrEmpty(sublocationId, nameof(sublocationId));
            if (recipientIds.IsNullOrEmpty())
            {
                throw new ArgumentException(@"recipientIds is null or empty", nameof(recipientIds));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    x => x.OwnerOdsCode == externalOrgId
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationId)
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .FirstOrDefaultAsync();

            if (sublocation.Competition.Completed.HasValue)
            {
                throw new InvalidOperationException(
                    "Cannot add recipients to sublocations on a completed competition.");
            }

            var anyIdAlreadyInServiceRecipients =
                recipientIds.All(x => sublocation.SublocationRecipients.Any(y => y.RecipientOdsCode == x));

            if (anyIdAlreadyInServiceRecipients)
            {
                throw new InvalidOperationException("One or more requested Ids already present in sublocation.");
            }

            IEnumerable<ServiceRecipient> validRecipientsForSublocation =
                await odsService.GetServiceRecipientsBySublocation(sublocationId);

            var allIdsValid = recipientIds.All(x => validRecipientsForSublocation.Any(y => y.OrgId == x));

            if (!allIdsValid)
            {
                throw new InvalidOperationException(
                    "One or more requested Ids not found or not valid for this sublocation.");
            }

            foreach (var recipientId in recipientIds)
            {
                sublocation.SublocationRecipients.Add(
                    new CompetitionSublocationRecipient
                    {
                        CompetitionId = competitionId,
                        RecipientOdsCode = recipientId,
                        ParentSublocationOdsCode = sublocationId,
                    });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationId,
            HashSet<string> recipientIds)
        {
            ArgumentException.ThrowIfNullOrEmpty(externalOrgId, nameof(externalOrgId));
            ArgumentException.ThrowIfNullOrEmpty(sublocationId, nameof(sublocationId));
            if (recipientIds.IsNullOrEmpty())
            {
                throw new ArgumentException(@"recipientIds is null or empty", nameof(recipientIds));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    x => x.OwnerOdsCode == externalOrgId
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationId)
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .FirstOrDefaultAsync();

            if (sublocation.Competition.Completed.HasValue)
            {
                throw new InvalidOperationException(
                    "Cannot remove recipients from sublocations on a completed competition.");
            }

            var anyIdAlreadyInServiceRecipients =
                recipientIds.All(x => sublocation.SublocationRecipients.Any(y => y.RecipientOdsCode == x));

            if (!anyIdAlreadyInServiceRecipients)
            {
                throw new InvalidOperationException("Can only remove recipient if present in sublocation.");
            }

            foreach (var recipientId in recipientIds)
            {
                CompetitionSublocationRecipient itemToRemove =
                    sublocation.SublocationRecipients.First(x => x.RecipientOdsCode == recipientId);

                sublocation.SublocationRecipients.Remove(itemToRemove);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
