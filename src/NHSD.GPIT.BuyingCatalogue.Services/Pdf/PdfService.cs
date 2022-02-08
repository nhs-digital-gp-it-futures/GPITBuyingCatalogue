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
            string htmlFilePath = string.Empty;
            string pdfFilePath = string.Empty;

            try
            {
                htmlFilePath = await ConvertToHtml(url);

                pdfFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

                HtmlConverter.ConvertToPdf(
                    new FileInfo(htmlFilePath),
                    new FileInfo(pdfFilePath));

                byte[] fileContent = File.ReadAllBytes(pdfFilePath);

                return fileContent;
            }
            finally
            {
                if (!string.IsNullOrEmpty(pdfFilePath) && File.Exists(pdfFilePath))
                    File.Delete(pdfFilePath);

                if (!string.IsNullOrEmpty(htmlFilePath) && File.Exists(htmlFilePath))
                    File.Delete(htmlFilePath);
            }
        }

        private static async Task<string> ConvertToHtml(Uri url)
        {
            string htmlFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.html");

            using (var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                },
            })
            {
                using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = url };
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                await using var ms = await response.Content.ReadAsStreamAsync();
                await using var fs = File.Create(htmlFilePath);
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(fs);
            }

            return htmlFilePath;
        }
    }
}
