using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AssociatedServiceModel
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public string OrderGuidance { get; set; }

        public IList<string> Prices { get; set; }
    }
}
