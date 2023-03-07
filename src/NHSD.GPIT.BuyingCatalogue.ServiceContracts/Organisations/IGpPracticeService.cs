using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeService
    {
        Task ImportGpPracticeData(Uri csvUri, string emailAddress);

        Task<int?> GetNumberOfPatients(string odsCode);
    }
}
