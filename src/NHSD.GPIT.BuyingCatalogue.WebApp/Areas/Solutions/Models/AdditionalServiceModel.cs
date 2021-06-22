using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AdditionalServiceModel
    {
        public string SolutionId { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public IList<string> Prices { get; set; }
    }
}
