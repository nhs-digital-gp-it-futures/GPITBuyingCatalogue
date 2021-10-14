using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public sealed class OrganisationsService : IOrganisationsService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<Organisation, BuyingCatalogueDbContext> organisationRepository;

        public OrganisationsService(
            BuyingCatalogueDbContext dbContext,
            IDbRepository<Organisation, BuyingCatalogueDbContext> organisationRepository)
        {
            this.organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IList<Organisation>> GetAllOrganisations()
        {
            return (await organisationRepository.GetAllAsync(o => true)).OrderBy(o => o.Name).ToList();
        }

        public async Task<(int OrganisationId, string Error)> AddOdsOrganisation(OdsOrganisation odsOrganisation, bool agreementSigned)
        {
            if (odsOrganisation is null)
                throw new ArgumentNullException(nameof(odsOrganisation));

            var persistedOrganisation = await dbContext.Organisations.FirstOrDefaultAsync(o => o.OdsCode == odsOrganisation.OdsCode);

            if (persistedOrganisation is not null)
                return (0, $"The organisation with ODS code {odsOrganisation.OdsCode} already exists.");

            var organisation = new Organisation
            {
                Address = odsOrganisation.Address,
                CatalogueAgreementSigned = agreementSigned,
                LastUpdated = DateTime.UtcNow,
                Name = odsOrganisation.OrganisationName,
                OdsCode = odsOrganisation.OdsCode.ToUpperInvariant(),
                PrimaryRoleId = odsOrganisation.PrimaryRoleId,
            };

            dbContext.Organisations.Add(organisation);
            await dbContext.SaveChangesAsync();

            return (organisation.Id, null);
        }

        public async Task<Organisation> GetOrganisation(int id)
        {
            return await organisationRepository.SingleAsync(o => o.Id == id);
        }

        public async Task<Organisation> GetOrganisationByOdsCode(string odsCode)
        {
            return await organisationRepository.SingleAsync(o => o.OdsCode == odsCode);
        }

        public async Task<List<Organisation>> GetOrganisationsByOdsCodes(string[] odsCodes)
        {
            return (await organisationRepository.GetAllAsync(o => odsCodes.Contains(o.OdsCode))).OrderBy(o => o.Name).ToList();
        }

        public async Task UpdateCatalogueAgreementSigned(int organisationId, bool signed)
        {
            var organisation = await organisationRepository.SingleAsync(o => o.Id == organisationId);
            organisation.CatalogueAgreementSigned = signed;
            await organisationRepository.SaveChangesAsync();
        }

        public async Task<List<Organisation>> GetUnrelatedOrganisations(int organisationId)
        {
            return await dbContext.Organisations
                 .Where(o =>
                    o.Id != organisationId
                    && o.RelatedOrganisationRelatedOrganisationNavigations.All(roron => roron.OrganisationId != organisationId))
                 .ToListAsync();
        }

        public async Task<List<Organisation>> GetRelatedOrganisations(int organisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(o => o.RelatedOrganisationOrganisations)
                .ThenInclude(ro => ro.RelatedOrganisationNavigation)
                .SingleAsync(o => o.Id == organisationId);

            return organisation.RelatedOrganisationOrganisations.Select(ro => ro.RelatedOrganisationNavigation).OrderBy(o => o.Name).ToList();
        }

        public async Task AddRelatedOrganisations(int organisationId, int relatedOrganisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(o => o.RelatedOrganisationOrganisations)
                .ThenInclude(ro => ro.RelatedOrganisationNavigation)
                .SingleAsync(o => o.Id == organisationId);

            if (organisation.RelatedOrganisationOrganisations.Any(ro => ro.RelatedOrganisationId == relatedOrganisationId))
                return;

            dbContext.RelatedOrganisations.Add(new RelatedOrganisation { OrganisationId = organisationId, RelatedOrganisationId = relatedOrganisationId });

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveRelatedOrganisations(int organisationId, int relatedOrganisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(o => o.RelatedOrganisationOrganisations)
                .ThenInclude(ro => ro.RelatedOrganisationNavigation)
                .SingleAsync(o => o.Id == organisationId);

            var relatedItem = organisation.RelatedOrganisationOrganisations.FirstOrDefault(ro => ro.RelatedOrganisationId == relatedOrganisationId);

            if (relatedItem is null)
                return;

            organisation.RelatedOrganisationOrganisations.Remove(relatedItem);

            await dbContext.SaveChangesAsync();
        }
    }
}
