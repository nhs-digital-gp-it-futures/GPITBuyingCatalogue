using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class AssociatedServiceModel
    {
        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public IEnumerable<string> Prices { get; set; }
    }
}
