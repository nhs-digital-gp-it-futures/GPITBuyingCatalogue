using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class OrganisationsService : IOrganisationsService
    {
        private readonly ILogWrapper<OrganisationsService> logger;
        private readonly UsersDbContext dbContext;
        private readonly IUsersDbRepository<Organisation> organisationRepository;

        public OrganisationsService(
            ILogWrapper<OrganisationsService> logger,
            UsersDbContext dbContext,
            IUsersDbRepository<Organisation> organisationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Organisation>> GetAllOrganisations()
        {
            return (await organisationRepository.GetAllAsync(x => true)).OrderBy(x => x.Name).ToList();
        }

        public async Task<Guid> AddOdsOrganisation(OdsOrganisation odsOrganisation, bool agreementSigned)
        {
            var organisation = new Organisation
            {
                Address = JsonConvert.SerializeObject(odsOrganisation.Address),
                OrganisationId = Guid.NewGuid(),
                CatalogueAgreementSigned = agreementSigned,
                LastUpdated = DateTime.UtcNow,
                Name = odsOrganisation.OrganisationName,
                OdsCode = odsOrganisation.OdsCode.ToUpper(),
                PrimaryRoleId = odsOrganisation.PrimaryRoleId,
            };

            organisationRepository.Add(organisation);

            await organisationRepository.SaveChangesAsync();

            return organisation.OrganisationId;
        }

        public async Task<Organisation> GetOrganisation(Guid id)
        {
            return await organisationRepository.SingleAsync(x => x.OrganisationId == id);
        }

        public async Task UpdateCatalogueAgreementSigned(Guid organisationId, bool signed)
        {
            var organisation = await organisationRepository.SingleAsync(x => x.OrganisationId == organisationId);
            organisation.CatalogueAgreementSigned = signed;
            await organisationRepository.SaveChangesAsync();
        }

        public async Task<List<Organisation>> GetUnrelatedOrganisations(Guid organisationId)
        {
            // TODO - should be able to combine this into a single query
            var allOrganisations = await GetAllOrganisations();
            var relatedOrganisations = await GetRelatedOrganisations(organisationId);
            return allOrganisations.Where(x => !relatedOrganisations.Any(y => y.OrganisationId == x.OrganisationId)).OrderBy(x => x.Name).ToList();
        }

        public async Task<List<Organisation>> GetRelatedOrganisations(Guid organisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(x => x.RelatedOrganisationOrganisations)
                .ThenInclude(x => x.RelatedOrganisationNavigation)
                .SingleAsync(x => x.OrganisationId == organisationId);

            return organisation.RelatedOrganisationOrganisations.Select(x => x.RelatedOrganisationNavigation).OrderBy(x => x.Name).ToList();
        }

        public async Task AddRelatedOrganisations(Guid organisationId, Guid relatedOrganisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(x => x.RelatedOrganisationOrganisations)
                .ThenInclude(x => x.RelatedOrganisationNavigation)
                .SingleAsync(x => x.OrganisationId == organisationId);

            if (organisation.RelatedOrganisationOrganisations.Any(x => x.RelatedOrganisationId == relatedOrganisationId))
                return;

            dbContext.RelatedOrganisations.Add(new RelatedOrganisation { OrganisationId = organisationId, RelatedOrganisationId = relatedOrganisationId });

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveRelatedOrganisations(Guid organisationId, Guid relatedOrganisationId)
        {
            var organisation = await dbContext.Organisations
                .Include(x => x.RelatedOrganisationOrganisations)
                .ThenInclude(x => x.RelatedOrganisationNavigation)
                .SingleAsync(x => x.OrganisationId == organisationId);

            var relatedItem = organisation.RelatedOrganisationOrganisations.FirstOrDefault(x => x.RelatedOrganisationId == relatedOrganisationId);

            if (relatedItem == null)
                return;

            organisation.RelatedOrganisationOrganisations.Remove(relatedItem);

            await dbContext.SaveChangesAsync();
        }
    }
}
