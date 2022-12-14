using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class ServiceModel
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public string Description { get; set; }

        public bool IsSelected { get; set; }
    }
}
