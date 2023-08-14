using System;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Extensions;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.IncrementalUpdate.Services
{
    public class IncrementalUpdateService : IIncrementalUpdateService
    {
        private readonly ILogger<IncrementalUpdateService> _logger;
        private readonly IOdsService _odsService;
        private readonly IOrganisationUpdateService _organisationUpdateService;

        public IncrementalUpdateService(
            ILogger<IncrementalUpdateService> logger,
            IOdsService odsService,
            IOrganisationUpdateService organisationUpdateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            _organisationUpdateService = organisationUpdateService ?? throw new ArgumentNullException(nameof(organisationUpdateService));
        }

        public async Task UpdateOrganisationData()
        {
            var lastRunDate = await _organisationUpdateService.GetLastRunDate();
            var organisationIds = (await _odsService.SearchByLastChangeDate(lastRunDate)).ToList();

            if (!organisationIds.Any())
            {
                _logger.LogInformation("No updates retrieved from ODS service");
                return;
            }

            var data = new IncrementalUpdateData
            {
                Relationships = (await _odsService.GetRelationships()).ToList(),
                Roles = (await _odsService.GetRoles()).ToList(),
            };

            foreach (var organisationId in organisationIds)
                data.Organisations.Add(await _odsService.GetOrganisation(organisationId));

            data.Organisations.RemoveAll(x => x == null);

            var relatedOrganisationIds = data.Organisations.RelatedOrganisationIds().Except(organisationIds);

            foreach (var organisationId in relatedOrganisationIds)
                data.RelatedOrganisations.Add(await _odsService.GetOrganisation(organisationId));

            data.RelatedOrganisations.RemoveAll(x => x == null);

            await _organisationUpdateService.IncrementalUpdate(data);
            await _organisationUpdateService.SetLastRunDate(DateTime.Today);

            _logger.LogInformation("Updated {Count} organisations", organisationIds.Count);
        }
    }
}
