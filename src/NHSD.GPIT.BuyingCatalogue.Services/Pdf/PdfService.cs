using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using iText.Html2pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        public async Task<byte[]> Convert(Uri url)
        {
            string outputFilePath = @$"{Path.GetTempPath()}{Guid.NewGuid()}.pdf";
            string inputFilePath = @$"{Path.GetTempPath()}{Guid.NewGuid()}.html";

            using (var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                },
            })
            {
                using (var httpClient = new HttpClient(httpClientHandler) { BaseAddress = url })
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    await using var ms = await response.Content.ReadAsStreamAsync();
                    await using var fs = File.Create(inputFilePath);
                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(fs);
                }
            }

            HtmlConverter.ConvertToPdf(
                new FileInfo(inputFilePath),
                new FileInfo(outputFilePath));

            byte[] fileContent = File.ReadAllBytes(outputFilePath);

            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            if (File.Exists(inputFilePath))
                File.Delete(inputFilePath);

            return fileContent;
        }
    }
}
