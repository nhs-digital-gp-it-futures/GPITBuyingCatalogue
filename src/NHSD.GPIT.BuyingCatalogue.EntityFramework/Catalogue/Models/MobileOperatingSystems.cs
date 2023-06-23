using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [ExcludeFromCodeCoverage]
    public class MobileOperatingSystems
    {
        public HashSet<string> OperatingSystems { get; set; } = new();

        public string OperatingSystemsDescription { get; set; }
    }
}
