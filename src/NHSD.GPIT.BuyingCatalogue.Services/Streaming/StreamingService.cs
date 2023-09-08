using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Streaming;

namespace NHSD.GPIT.BuyingCatalogue.Services.Streaming
{
    [ExcludeFromCodeCoverage(Justification = "HTTP and ZIP interaction")]
    public class StreamingService : IStreamingService
    {
        private const string CsvExtension = ".csv";
        private const string ZipExtension = ".zip";
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

            if (!response.IsSuccessStatusCode) return null;

            var contents = await response.Content.ReadAsStreamAsync();

            return !uri.AbsolutePath.Contains(ZipExtension, StringComparison.OrdinalIgnoreCase)
                ? contents
                : await UnwrapCsvFromZip(contents);
        }

        private static async Task<Stream> UnwrapCsvFromZip(Stream stream)
        {
            using var zipFile = new ZipFile(stream);

            var files = zipFile.Cast<ZipEntry>().Where(x => x.IsFile && x.Name.EndsWith(CsvExtension, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (files.Length is 0 or > 1) return null;

            var csvStream = zipFile.GetInputStream(files.First());

            var memoryStream = new MemoryStream();
            await csvStream.CopyToAsync(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
