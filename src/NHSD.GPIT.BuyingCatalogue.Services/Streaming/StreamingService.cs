using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Streaming;

namespace NHSD.GPIT.BuyingCatalogue.Services.Streaming
{
    public class StreamingService : IStreamingService
    {
        private readonly HttpClient httpClient;

        public StreamingService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stream> StreamContents(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            var response = await httpClient.GetAsync(uri);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStreamAsync()
                : null;
        }
    }
}
