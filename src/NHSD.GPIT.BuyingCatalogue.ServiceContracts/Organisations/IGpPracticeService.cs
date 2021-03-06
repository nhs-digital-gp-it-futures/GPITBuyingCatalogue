using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeService
    {
        Task ImportGpPracticeData(Uri csvUri, string emailAddress);

        Task SendConfirmationEmail(ImportGpPracticeListResult result, string emailAddress);
    }
}
