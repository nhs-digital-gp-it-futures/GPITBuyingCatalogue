using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf
{
    public interface IPdfService
    {
        byte[] Convert(string url);
    }
}
