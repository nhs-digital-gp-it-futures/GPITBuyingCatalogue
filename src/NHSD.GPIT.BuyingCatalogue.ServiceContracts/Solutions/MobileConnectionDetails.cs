using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class MobileConnectionDetails
    {
        public HashSet<string> ConnectionType { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string Description { get; set; }
    }
}
