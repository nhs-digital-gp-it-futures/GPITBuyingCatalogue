using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class OrganisationsService : IOrganisationsService
    {
        private readonly ILogWrapper<OrganisationsService> _logger;
        private readonly IUsersDbRepository<Organisation> _organisationRepository;

        public OrganisationsService(ILogWrapper<OrganisationsService> logger,
            IUsersDbRepository<Organisation> organisationRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
        }

        public async Task<List<Organisation>> GetAllOrganisations()
        {
            return (await _organisationRepository.GetAllAsync(x => true)).OrderBy(x=>x.Name).ToList();
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
                PrimaryRoleId = odsOrganisation.PrimaryRoleId
            };

            _organisationRepository.Add(organisation);

            await _organisationRepository.SaveChangesAsync();

            return organisation.OrganisationId;
        }

        public async Task<Organisation> GetOrganisation(Guid id)
        {
            return await _organisationRepository.SingleAsync(x => x.OrganisationId == id);
        }

        public async Task UpdateCatalogueAgreementSigned(Guid organisationId, bool signed)
        {
            var organisation =  await _organisationRepository.SingleAsync(x => x.OrganisationId == organisationId);

            organisation.CatalogueAgreementSigned = signed;

            await _organisationRepository.SaveChangesAsync();
        }
    }
}
