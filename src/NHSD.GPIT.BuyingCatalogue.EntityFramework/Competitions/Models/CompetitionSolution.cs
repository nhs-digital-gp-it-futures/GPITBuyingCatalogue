using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionSolution
{
    public CompetitionSolution()
    {
        RequiredServices = new HashSet<RequiredService>();
    }

    public CompetitionSolution(int competitionId, CatalogueItemId solutionId)
        : this()
    {
        CompetitionId = competitionId;
        SolutionId = solutionId;
    }

    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public bool IsShortlisted { get; set; }

    public string Justification { get; set; }

    public Solution Solution { get; set; }

    public ICollection<RequiredService> RequiredServices { get; set; } = new HashSet<RequiredService>();

    public ICollection<SolutionScore> Scores { get; set; } = new HashSet<SolutionScore>();

    public bool HasScoreType(ScoreType type) => Scores.Any(x => x.ScoreType == type);

    public SolutionScore GetScoreByType(ScoreType type) => Scores?.FirstOrDefault(x => x.ScoreType == type);
}
