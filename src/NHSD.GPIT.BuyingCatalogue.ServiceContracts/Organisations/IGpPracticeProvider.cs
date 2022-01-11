using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeProvider
    {
        Task<IEnumerable<GpPractice>> GetGpPractices(Uri csvUri);
    }
}
