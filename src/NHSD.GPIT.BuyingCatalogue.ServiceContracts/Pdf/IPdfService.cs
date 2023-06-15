using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf
{
    public interface IPdfService
    {
        Task<byte[]> Convert(System.Uri url);

        Uri BaseUri();
    }
}
