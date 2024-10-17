using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.OrganisationImport.Services;

public class HttpService(
    IHttpClientFactory httpClientFactory,
    ILogger<HttpService> logger)
    : IHttpService
{
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ILogger<HttpService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Stream> DownloadAsync(Uri url)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();

            var responseBody = await response.Content.ReadAsStringAsync();
            logger.LogError(
                "Failed to download TRUD dataset.\r\nStatus Code: {StatusCode}\r\nError: {ResponseContent}",
                response.StatusCode, responseBody);

            return null;

        }
        catch (HttpRequestException reqEx)
        {
            logger.LogCritical("Caught HTTP Request Exception\r\nStatus Code: {StatusCode}\r\nMessage: {Message}",
                reqEx.StatusCode, reqEx.Message);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            logger.LogCritical("Request timed out: {Message}", ex.Message);
            throw;
        }
    }
}
