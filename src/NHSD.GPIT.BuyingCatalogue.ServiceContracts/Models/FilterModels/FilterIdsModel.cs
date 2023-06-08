using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public class FilterIdsModel
    {
        public IEnumerable<int> CapabilityIds { get; set; }

        public IEnumerable<string> EpicIds { get; set; }

        public string FrameworkId { get; set; }

        public IEnumerable<int> ClientApplicationTypeIds { get; set; }

        public IEnumerable<int> HostingTypeIds { get; set; }
    }
}
