using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<FrameworkFilterInfo>> GetFrameworksByCatalogueItems(IList<CatalogueItem> catalogueItems);

        public Task<Framework> GetFrameworksById(string frameworkId);
    }
}
