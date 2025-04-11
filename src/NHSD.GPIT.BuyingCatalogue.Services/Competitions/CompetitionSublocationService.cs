using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
            return await dbContext
                .CompetitionSublocations
                .AsNoTracking()
                .Where(
                    CompetitionSublocationPrimaryKeyPredicate(externalOrgId, competitionId, sublocationOdsCode))
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .ThenInclude(y => y.RecipientOrganisation)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCountForCompetitionSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationOdsCode)
        {
            return await dbContext.CompetitionSublocations
                .Where(CompetitionSublocationPrimaryKeyPredicate(externalOrgId, competitionId, sublocationOdsCode))
                .Include(x => x.SublocationRecipients)
                .SelectMany(s => s.SublocationRecipients)
                .CountAsync();
        }

        public async Task AddSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentOdsCode);
            ArgumentException.ThrowIfNullOrEmpty(sublocationOdsCode);
            if (recipientOdsCodes is null or { Count: 0 })
            {
                throw new ArgumentException(@"recipientOdsCodes is null or empty", nameof(recipientOdsCodes));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    CompetitionSublocationPrimaryKeyPredicate(parentOdsCode, competitionId, sublocationOdsCode))
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .FirstAsync();

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

            sublocation.SublocationRecipients.AddRange(
                recipientOdsCodes.Select(
                    x => new CompetitionSublocationRecipient
                    {
                        CompetitionId = competitionId,
                        RecipientOdsCode = x,
                        ParentSublocationOdsCode = sublocationOdsCode,
                    }));

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentOdsCode);
            ArgumentException.ThrowIfNullOrEmpty(sublocationOdsCode);
            if (recipientOdsCodes is null or { Count: 0 })
            {
                throw new ArgumentException(@"recipientOdsCodes is null or empty", nameof(recipientOdsCodes));
            }

            CompetitionSublocation sublocation = await dbContext
                .CompetitionSublocations
                .Where(
                    CompetitionSublocationPrimaryKeyPredicate(parentOdsCode, competitionId, sublocationOdsCode))
                .Include(x => x.Competition)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .FirstOrDefaultAsync();

            if (sublocation.Competition.Completed.HasValue)
            {
                throw new InvalidOperationException(
                    "Cannot remove recipients from sublocations on a completed competition.");
            }

            List<CompetitionSublocationRecipient> itemsToRemove =
                sublocation.SublocationRecipients.Where(x => recipientOdsCodes.Contains(x.RecipientOdsCode)).ToList();

            sublocation.SublocationRecipients.RemoveRange(itemsToRemove);

            await dbContext.SaveChangesAsync();
        }

        private static Expression<Func<CompetitionSublocation, bool>> CompetitionSublocationPrimaryKeyPredicate(
            string externalOrgId,
            int competitionId,
            string sublocationOdsCode)
        {
            return x => x.OwnerOdsCode == externalOrgId
                && x.CompetitionId == competitionId
                && x.SublocationOdsCode == sublocationOdsCode;
        }
    }
}
