using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        private const string ChromeArgs = "--no-sandbox --headless --disable-gpu --disable-software-rasterizer --ignore-certificate-errors";
        private const string ChromeWindowsPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        private const string ChromeLinuxPath = "/usr/bin/chromium-browser";

        public byte[] Convert(Uri url)
        {
            string fileName;
            string chromePath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = @$"{Path.GetTempPath()}{Guid.NewGuid()}.pdf";
                chromePath = ChromeWindowsPath;
            }
            else
            {
                fileName = @$"/tmp/{Guid.NewGuid()}.pdf";
                chromePath = ChromeLinuxPath;
            }

            var psi = new ProcessStartInfo(chromePath, $"{ChromeArgs} --print-to-pdf={fileName} {url}");
            var process = Process.Start(psi);
            process.WaitForExit(10000);

            byte[] fileContent = File.ReadAllBytes(fileName);

            if (File.Exists(fileName))
                File.Delete(fileName);

            return fileContent;
        }
    }
}
