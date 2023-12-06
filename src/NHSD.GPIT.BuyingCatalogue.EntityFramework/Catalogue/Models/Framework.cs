using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class Framework : IAudited
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public bool HasSingleFundingType => FundingTypes.Count == 1;

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public bool IsExpired { get; set; }

        public ICollection<FundingType> FundingTypes { get; set; } = new HashSet<FundingType>();

        public bool SupportsFoundationSolutions { get; set; }
    }
}
