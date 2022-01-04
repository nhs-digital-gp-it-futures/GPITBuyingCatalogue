using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public interface INoNavModel
    {
        DateTime LastReviewed { get; set; }

        string Name { get; set; }

        CatalogueItemId SolutionId { get; set; }

        string SolutionName { get; set; }
    }
}
