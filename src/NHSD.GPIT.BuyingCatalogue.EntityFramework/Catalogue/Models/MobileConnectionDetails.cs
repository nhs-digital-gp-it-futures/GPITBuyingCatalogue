using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class MobileConnectionDetails
    {
        public HashSet<string> ConnectionType { get; set; }

        public string Description { get; set; }

        public string MinimumConnectionSpeed { get; set; }
    }
}
