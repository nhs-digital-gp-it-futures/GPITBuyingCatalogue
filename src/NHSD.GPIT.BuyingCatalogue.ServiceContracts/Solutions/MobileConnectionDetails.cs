using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class MobileConnectionDetails
    {
        public HashSet<string> ConnectionType { get; set; }

        public string Description { get; set; }

        public string MinimumConnectionSpeed { get; set; }
    }
}
