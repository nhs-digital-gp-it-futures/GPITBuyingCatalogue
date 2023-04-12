using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeService
    {
        Task ImportGpPracticeData(Uri csvUri, string emailAddress);

        Task<IList<GpPracticeSize>> GetNumberOfPatients(IEnumerable<string> odsCodes);
    }
}
