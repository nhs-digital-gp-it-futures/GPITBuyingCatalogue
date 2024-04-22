using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<FrameworkFilterInfo>> GetFrameworksWithPublishedCatalogueItems();

        public Task<Framework> GetFramework(string frameworkId);

        public Task<Framework> GetFrameworkByName(string frameworkName);

        Task<IList<Framework>> GetFrameworks();

        Task AddFramework(string name, IEnumerable<FundingType> fundingTypes, int maximumTerm);

        Task UpdateFramework(string frameworkId, string name, IEnumerable<FundingType> fundingTypes, int maximumTerm);

        Task MarkAsExpired(string frameworkId);

        Task<bool> FrameworkNameExists(string frameworkName);

        Task<bool> FrameworkNameExistsExcludeSelf(string frameworkName, string frameworkId);
    }
}
