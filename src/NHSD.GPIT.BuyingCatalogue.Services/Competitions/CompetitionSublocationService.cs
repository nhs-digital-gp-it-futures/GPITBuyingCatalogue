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
            string sublocationOdsCode)
        {
            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .AsNoTracking()
                .Where(
                    x => x.OwnerOdsCode == externalOrgId
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationOdsCode)
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .ThenInclude(y => y.RecipientOrganisation)
                .FirstAsync();
            return sublocation;
        }

        public async Task AddSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentOdsCode, nameof(parentOdsCode));
            ArgumentException.ThrowIfNullOrEmpty(sublocationOdsCode, nameof(sublocationOdsCode));
            if (recipientOdsCodes.IsNullOrEmpty())
            {
                throw new ArgumentException(@"recipientIds is null or empty", nameof(recipientOdsCodes));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    x => x.OwnerOdsCode == parentOdsCode
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationOdsCode)
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
                recipientOdsCodes.All(x => sublocation.SublocationRecipients.Any(y => y.RecipientOdsCode == x));

            if (anyIdAlreadyInServiceRecipients)
            {
                throw new InvalidOperationException("One or more requested Ids already present in sublocation.");
            }

            IEnumerable<ServiceRecipient> validRecipientsForSublocation =
                await odsService.GetServiceRecipientsBySublocation(sublocationOdsCode);

            var allIdsValid = recipientOdsCodes.All(x => validRecipientsForSublocation.Any(y => y.OrgId == x));

            if (!allIdsValid)
            {
                throw new InvalidOperationException(
                    "One or more requested Ids not found or not valid for this sublocation.");
            }

            foreach (var recipientOdsCode in recipientOdsCodes)
            {
                sublocation.SublocationRecipients.Add(
                    new CompetitionSublocationRecipient
                    {
                        CompetitionId = competitionId,
                        RecipientOdsCode = recipientOdsCode,
                        ParentSublocationOdsCode = sublocationOdsCode,
                    });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(externalOrgId, nameof(externalOrgId));
            ArgumentException.ThrowIfNullOrEmpty(sublocationOdsCode, nameof(sublocationOdsCode));
            if (recipientOdsCodes.IsNullOrEmpty())
            {
                throw new ArgumentException(@"recipientIds is null or empty", nameof(recipientOdsCodes));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    x => x.OwnerOdsCode == externalOrgId
                        && x.CompetitionId == competitionId
                        && x.SublocationOdsCode == sublocationOdsCode)
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
                recipientOdsCodes.All(x => sublocation.SublocationRecipients.Any(y => y.RecipientOdsCode == x));

            if (!anyIdAlreadyInServiceRecipients)
            {
                throw new InvalidOperationException("Can only remove recipient if present in sublocation.");
            }

            foreach (var recipientOdsCode in recipientOdsCodes)
            {
                CompetitionSublocationRecipient itemToRemove =
                    sublocation.SublocationRecipients.First(x => x.RecipientOdsCode == recipientOdsCode);

                sublocation.SublocationRecipients.Remove(itemToRemove);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
