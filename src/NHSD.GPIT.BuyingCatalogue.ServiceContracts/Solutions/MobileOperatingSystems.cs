using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class MobileOperatingSystems
    {
        public HashSet<string> OperatingSystems { get; set; } = new();

        public string OperatingSystemsDescription { get; set; }
    }
}
