using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class SolutionStandard
{
    public SolutionStandard()
    {
    }

    public SolutionStandard(
        string standardId,
        StandardCompliance status)
    {
        StandardId = standardId;
        Status = status;
    }

    public CatalogueItemId SolutionId { get; set; }

    [StringLength(5)]
    public string StandardId { get; set; }

    public StandardCompliance Status { get; set; }

    public Standard Standard { get; set; }
}
