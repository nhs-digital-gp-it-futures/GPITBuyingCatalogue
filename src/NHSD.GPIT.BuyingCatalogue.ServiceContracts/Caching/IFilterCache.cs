using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching
{
    public interface IFilterCache
    {
        string Get(string filterKey);

        void Set(string filterKey, string content, DateTime expiration);

        void Remove(string filterKey);

        void Remove(IEnumerable<string> filterKeys);
    }
}
