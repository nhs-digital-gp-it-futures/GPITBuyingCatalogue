using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using BuyingCatalogueFunction.Settings;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.IncrementalUpdate.Services
{
    public class OdsService : IOdsService
    {
        private const string HttpStatusPattern = "3xx,4xx";

        private readonly ILogger<OdsService> _logger;
        private readonly OdsSettings _settings;

        public OdsService(ILogger<OdsService> logger, OdsSettings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<Org> GetOrganisation(string orgId)
        {
            var response = await _settings.OrganisationsUri
                .AppendPathSegment(orgId)
                .AllowHttpStatus(HttpStatusPattern)
                .GetJsonAsync<OrganisationResponse>();

            if (response == null)
                _logger.LogWarning("Failed to retrieve ODS organisation data for id {OrganisationId}", orgId);

            return response?.Organisation;
        }

        public async Task<IEnumerable<Relationship>> GetRelationships()
        {
            var response = await _settings.RelationshipsUri
                .AllowHttpStatus(HttpStatusPattern)
                .GetJsonAsync<RelationshipsResponse>();

            if (response == null)
                _logger.LogWarning("Failed to retrieve ODS relationship data");

            return response?.Relationships ?? Enumerable.Empty<Relationship>();
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            var response = await _settings.RolesUri
                .AllowHttpStatus(HttpStatusPattern)
                .GetJsonAsync<RolesResponse>();

            if (response == null)
                _logger.LogWarning("Failed to retrieve ODS role data");

            return response?.Roles ?? Enumerable.Empty<Role>();
        }

        public async Task<IEnumerable<string>> SearchByLastChangeDate(DateTime lastChangedDate)
        {
            var formattedDate = $"{lastChangedDate:yyyy-MM-dd}";

            var response = await _settings.SearchUri
                .SetQueryParam("LastChangeDate", formattedDate)
                .AllowHttpStatus(HttpStatusPattern)
                .GetJsonAsync<SearchResponse>();

            if (response == null)
                _logger.LogWarning("Failed to retrieve ODS updates for last change date {LastChangeDate}", formattedDate);

            return response?.OrganisationIds ?? Enumerable.Empty<string>();
        }
    }
}
