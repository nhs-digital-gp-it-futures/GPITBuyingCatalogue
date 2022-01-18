using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        public byte[] Convert(string url)
        {
            // MJRTODO - Maybe make cert ignore configurable??

            var fileName = string.Empty;
            string args = string.Empty;
            string chrome = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = @$"C:\Temp\{Guid.NewGuid()}.pdf";
                args = @$"--headless --disable-gpu --ignore-certificate-errors --print-to-pdf={fileName} {url}";
                chrome = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            }
            else
            {
                fileName = @$"/tmp/{Guid.NewGuid()}.pdf";
                args = $"--no-sandbox --headless --disable-gpu --disable-software-rasterizer --ignore-certificate-errors --print-to-pdf={fileName} {url}";
                chrome = "/usr/bin/chromium-browser";
            }

            ProcessStartInfo psi = new ProcessStartInfo(chrome, args);
            Process process = Process.Start(psi);
            process.WaitForExit(10000);

            byte[] fileContent = File.ReadAllBytes(fileName);

            File.Delete(fileName);

            return fileContent;
        }
    }
}
