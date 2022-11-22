using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.IncrementalUpdate;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.Settings;
using Flurl;
using Flurl.Http;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate
{
    public class OdsService : IOdsService
    {
        private readonly OdsSettings _settings;

        public OdsService(OdsSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<SearchResult> SearchByLastChangedDate(DateTime lastChangedDate)
        {
            var formattedDate = $"{lastChangedDate:yyyy-MM-dd}";

            return await _settings.SearchUri
                .SetQueryParam("LastChangeDate", formattedDate)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<SearchResult>();
        }

        public async Task<Organisation> GetOrganisation(string orgId)
        {
            var result = await _settings.OrganisationsUri
                .AppendPathSegment(orgId)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<LookupResult>();

            return result.Organisation;
        }
    }
}
