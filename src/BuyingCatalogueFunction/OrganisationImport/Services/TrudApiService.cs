using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using BuyingCatalogueFunction.OrganisationImport.Models;
using Microsoft.Extensions.Options;

namespace BuyingCatalogueFunction.OrganisationImport.Services;

public class TrudApiService(
    IHttpClientFactory httpClientFactory,
    IOptions<TrudApiOptions> trudApiOptions)
    : ITrudApiService
{
    private readonly HttpClient httpClientFactory = httpClientFactory.CreateClient(nameof(TrudApiService));

    private readonly TrudApiOptions trudApiOptions =
        trudApiOptions?.Value ?? throw new ArgumentNullException(nameof(trudApiOptions));

    public async Task<TrudApiResponse.Release> GetLatestReleaseInfo()
    {
        var response = await httpClientFactory.GetStringAsync($"trud/api/v1/keys/{trudApiOptions.ApiKey}/items/{trudApiOptions.ItemId}/releases?latest");

        var responseResult = JsonSerializer.Deserialize<TrudApiResponse>(response);

        return responseResult.Releases.FirstOrDefault();
    }
}
