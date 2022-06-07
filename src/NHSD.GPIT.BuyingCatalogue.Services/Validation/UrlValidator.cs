using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;

namespace NHSD.GPIT.BuyingCatalogue.Services.Validation
{
    public sealed class UrlValidator : IUrlValidator
    {
        private readonly HttpClient httpClient;

        public UrlValidator(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "URL as string is intentional")]
        public bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url cannot be null, empty or whitespace", nameof(url));

            if (!Uri.TryCreate(url, UriKind.Absolute, out var parsedUri))
                return false;

            try
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, parsedUri.GetLeftPart(UriPartial.Path));
                httpRequestMessage.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("BuyingCatalogue", "1.0"));

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                using var response = httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token)
                                        .GetAwaiter()
                                        .GetResult();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                return false;
            }
        }
    }
}
