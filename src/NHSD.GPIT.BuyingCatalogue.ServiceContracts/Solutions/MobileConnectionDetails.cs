using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class MobileConnectionDetails
    {
        public HashSet<string> ConnectionType { get; set; }

        public string Description { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public bool? IsValid() =>
            !string.IsNullOrWhiteSpace(MinimumConnectionSpeed) ||
            !string.IsNullOrWhiteSpace(Description)
                ? true
                : ConnectionType?.Any();
    }
}
