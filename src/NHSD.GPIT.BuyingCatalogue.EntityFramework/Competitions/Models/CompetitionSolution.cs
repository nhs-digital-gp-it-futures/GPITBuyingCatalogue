using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionSolution
{
    public CompetitionSolution()
    {
    }

    public CompetitionSolution(int competitionId, CatalogueItemId solutionId)
    {
        CompetitionId = competitionId;
        SolutionId = solutionId;
    }

    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public bool IsShortlisted { get; set; }

    public string Justification { get; set; }

    public Solution Solution { get; set; }

    public ICollection<RequiredService> RequiredServices { get; set; }
}
