using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServiceModel
    {
        public CatalogueItemId SolutionId { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public IList<string> Prices { get; set; }
    }
}
