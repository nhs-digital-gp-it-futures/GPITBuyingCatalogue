using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public interface INoNavModel
    {
        string LastReviewed { get; set; }

        string Name { get; set; }

        CatalogueItemId SolutionId { get; set; }

        string SolutionName { get; set; }
    }
}
