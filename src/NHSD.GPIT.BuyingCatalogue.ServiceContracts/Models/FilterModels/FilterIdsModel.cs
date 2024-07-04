using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public class FilterIdsModel
    {
        public Dictionary<int, string[]> CapabilityAndEpicIds { get; set; }

        public string FrameworkId { get; set; }

        public IEnumerable<int> ApplicationTypeIds { get; set; }

        public IEnumerable<int> HostingTypeIds { get; set; }

        public Dictionary<SupportedIntegrations, int[]> IntegrationsIds { get; set; }
    }
}
