using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<Framework>> GetFrameworksWithPublishedCatalogueItems();

        public Task<Framework> GetFramework(string frameworkId);

        Task<IList<Framework>> GetFrameworks();

        Task AddFramework(string name, IEnumerable<FundingType> fundingTypes, int maximumTerm);

        Task UpdateFramework(string frameworkId, string name, IEnumerable<FundingType> fundingTypes, int maximumTerm);

        Task MarkAsExpired(string frameworkId);

        Task<bool> FrameworkNameExists(string frameworkName);

        Task<bool> FrameworkNameExistsExcludeSelf(string frameworkName, string frameworkId);
    }
}
