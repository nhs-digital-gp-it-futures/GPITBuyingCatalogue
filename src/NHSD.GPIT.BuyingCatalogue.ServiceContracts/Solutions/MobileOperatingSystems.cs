using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class MobileOperatingSystems
    {
        public HashSet<string> OperatingSystems { get; set; } = new();

        public string OperatingSystemsDescription { get; set; }

        public TaskProgress Status() => (OperatingSystems?.Any() ?? false) ? TaskProgress.Completed : TaskProgress.NotStarted;
    }
}
