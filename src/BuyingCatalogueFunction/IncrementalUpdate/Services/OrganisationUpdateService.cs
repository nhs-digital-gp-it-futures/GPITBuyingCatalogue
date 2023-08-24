using System;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.IncrementalUpdate.Services
{
    public class OrganisationUpdateService : IOrganisationUpdateService
    {
        public const int MaxDays = -180;

        private readonly BuyingCatalogueDbContext _dbContext;
        private readonly IOdsOrganisationService _odsOrganisationService;

        public OrganisationUpdateService(
            BuyingCatalogueDbContext dbContext,
            IOdsOrganisationService odsOrganisationService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _odsOrganisationService = odsOrganisationService ?? throw new ArgumentNullException(nameof(odsOrganisationService));
        }

        public async Task<DateTime> GetLastRunDate()
        {
            var journal = await _dbContext.OrgImportJournal.FirstOrDefaultAsync();
            var lastRunDate = journal?.ImportDate ?? DateTime.Today.AddDays(-1);

            lastRunDate = lastRunDate < DateTime.Today.AddDays(MaxDays)
                ? DateTime.Today.AddDays(MaxDays)
                : lastRunDate;

            return lastRunDate;
        }

        public async Task SetLastRunDate(DateTime lastRunDate)
        {
            var journal = await _dbContext.OrgImportJournal.FirstOrDefaultAsync();

            if (journal == null)
                _dbContext.OrgImportJournal.Add(new OrgImportJournal
                {
                    ImportDate = lastRunDate,
                });
            else
                journal.ImportDate = lastRunDate;

            await _dbContext.SaveChangesAsync();
        }

        public async Task IncrementalUpdate(IncrementalUpdateData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var allOrganisations = data.Organisations.Union(data.RelatedOrganisations).ToList();

            IDbContextTransaction transaction = null;

            try
            {
                transaction = await _dbContext.Database.BeginTransactionAsync();

                await _odsOrganisationService.AddRelationshipTypes(data.Relationships);
                await _odsOrganisationService.AddRoleTypes(data.Roles);
                await _odsOrganisationService.UpsertOrganisations(allOrganisations);
                await _odsOrganisationService.AddOrganisationRelationships(data.Organisations);
                await _odsOrganisationService.AddOrganisationRoles(data.Organisations);

                await transaction.CommitAsync();
            }
            catch
            {
                if (transaction != null)
                    await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
