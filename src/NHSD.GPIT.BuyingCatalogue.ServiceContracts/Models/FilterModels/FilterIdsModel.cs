using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public class FilterIdsModel
    {
        public Dictionary<int, string[]> CapabilityAndEpicIds { get; set; }

        public string FrameworkId { get; set; }

        public IEnumerable<int> ApplicationTypeIds { get; set; }

        public IEnumerable<int> HostingTypeIds { get; set; }

        public IEnumerable<int> IM1Integrations { get; set; }

        public IEnumerable<int> GPConnectIntegrations { get; set; }

        public IEnumerable<int> InteroperabilityOptions { get; set; }
    }
}
