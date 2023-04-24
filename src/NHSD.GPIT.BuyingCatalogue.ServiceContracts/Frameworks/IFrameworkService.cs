using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<Framework>> GetFrameworks();

        public Task<Framework> GetFrameworksById(string frameworkId);
    }
}
