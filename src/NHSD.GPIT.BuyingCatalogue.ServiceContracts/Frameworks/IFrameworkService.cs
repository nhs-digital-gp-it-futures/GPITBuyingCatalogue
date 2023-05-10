using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<FrameworkFilterInfo>> GetFrameworksByCatalogueItems(IList<CatalogueItemId> catalogueItems);

        public Task<Framework> GetFramework(string frameworkId);

        Task<IList<Framework>> GetFrameworks();

        Task AddFramework(string name, bool isLocalFundingOnly);

        Task MarkAsExpired(string frameworkId);

        Task<bool> FrameworkNameExists(string frameworkName);
    }
}
