using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class InProgressSolutionStandard
{
    public InProgressSolutionStandard()
    {
    }

    public InProgressSolutionStandard(
        string standardId)
    {
        StandardId = standardId;
    }

    public InProgressSolutionStandard(
        CatalogueItemId solutionId,
        string standardId)
        : this(standardId)
    {
        SolutionId = solutionId;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string StandardId { get; set; }

    public Solution Solution { get; set; }

    public Standard Standard { get; set; }
}
