using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        public byte[] Convert(Uri url)
        {
            var rnd = new Random();
            var b = new byte[10];
#pragma warning disable CA5394 // Do not use insecure randomness
            rnd.NextBytes(b);
#pragma warning restore CA5394 // Do not use insecure randomness

            return b;
        }
    }
}
