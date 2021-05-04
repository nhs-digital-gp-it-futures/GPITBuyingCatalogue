using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
