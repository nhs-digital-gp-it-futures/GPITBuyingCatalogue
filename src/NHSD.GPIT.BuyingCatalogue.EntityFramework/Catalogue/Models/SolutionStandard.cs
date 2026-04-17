using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class SolutionStandard
{
    public CatalogueItemId SolutionId { get; set; }

    [StringLength(5)]
    public string StandardId { get; set; }

    public SolutionStandardStatus Status { get; set; }
}
