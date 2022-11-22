using Microsoft.Extensions.Logging;
using OrganisationImporter.Interfaces;

namespace OrganisationImporter.Services;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<HttpService> logger;

    public HttpService(
        IHttpClientFactory httpClientFactory,
        ILogger<HttpService> logger)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Stream> DownloadAsync(Uri url)
    {
        logger.LogInformation("Downloading TRUD data from {Url}", url);

        try
        {
            using var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                logger.LogError(
                    "Failed to download TRUD dataset.\r\nStatus Code: {StatusCode}\r\nError: {ResponseContent}",
                    response.StatusCode, responseBody);

                return null;
            }

            return await response.Content.ReadAsStreamAsync();
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
