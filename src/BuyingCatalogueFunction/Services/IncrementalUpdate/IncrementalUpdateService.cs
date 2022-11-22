using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate
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
            var response = await _odsService.SearchByLastChangedDate(DateTime.Today.AddDays(-3));

            var orgIds = response.Organisations
                .Select(x => x.OrgLink.Split("/").Last())
                .Distinct()
                .ToList();

            if (!orgIds.Any())
            {
                _logger.LogInformation("No changes retrieved from ODS service");
                return;
            }

            var organisations = new List<Organisation>();

            foreach (var organisationId in orgIds)
            {
                organisations.Add(await _odsService.GetOrganisation(organisationId));
            }

            var relationshipIds = organisations.SelectMany(x => x.Rels?.Rel ?? new List<Rel>())
                .Select(x => x.id)
                .Distinct()
                .ToList();

            await _organisationUpdateService.AddRelationshipTypes(relationshipIds);

            var roleIds = organisations.SelectMany(x => x.Roles?.Role ?? new List<Role>())
                .Select(x => x.id)
                .Distinct()
                .ToList();

            await _organisationUpdateService.AddRoleTypes(roleIds);

            foreach (var organisation in organisations)
            {
                await _organisationUpdateService.Upsert(organisation);
            }

            var relatedOrganisationIds = organisations.SelectMany(x => x.Rels?.Rel ?? new List<Rel>())
                .Select(x => x.Target.OrgId.extension)
                .Distinct()
                .ToList();

            await _organisationUpdateService.AddMissingOrganisations(relatedOrganisationIds);

            foreach (var organisation in organisations)
            {
                await _organisationUpdateService.AddRelationships(organisation);
                await _organisationUpdateService.AddRoles(organisation);
            }

            _logger.LogInformation($"Updated {orgIds.Count} organisations");
        }
    }
}
