using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeImportService
    {
        Task<ImportGpPracticeListResult> PerformImport(Uri csvUri);
    }
}
