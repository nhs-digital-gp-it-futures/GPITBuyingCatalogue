﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        private const string ChromeArgs = "--no-sandbox --headless --disable-dev-shm-usage --disable-gpu --disable-software-rasterizer --ignore-certificate-errors";
        private const string ChromeWindowsPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        private const string ChromeLinuxPath = "/usr/bin/chromium-browser";

        public byte[] Convert(Uri url)
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            string chromePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ChromeWindowsPath : ChromeLinuxPath;

            try
            {
                var psi = new ProcessStartInfo(chromePath, $"{ChromeArgs} --print-to-pdf={filePath} {url}");
                var process = Process.Start(psi);
                process.WaitForExit(10000);
                return File.ReadAllBytes(filePath);
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }
}
