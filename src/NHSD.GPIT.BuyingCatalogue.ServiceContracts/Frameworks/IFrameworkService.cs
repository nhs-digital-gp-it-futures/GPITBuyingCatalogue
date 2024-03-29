﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks
{
    public interface IFrameworkService
    {
        public Task<List<FrameworkFilterInfo>> GetFrameworksWithPublishedCatalogueItems();

        public Task<Framework> GetFramework(string frameworkId);

        Task<IList<Framework>> GetFrameworks();

        Task AddFramework(string name, IEnumerable<FundingType> fundingTypes, bool supportsFoundationSolution);

        Task UpdateFramework(string frameworkId, string name, IEnumerable<FundingType> fundingTypes, bool supportsFoundationSolution);

        Task MarkAsExpired(string frameworkId);

        Task<bool> FrameworkNameExists(string frameworkName);

        Task<bool> FrameworkNameExistsExcludeSelf(string frameworkName, string frameworkId);
    }
}
