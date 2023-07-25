﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class SolutionScore
{
    public SolutionScore()
    {
    }

    public SolutionScore(
        ScoreType scoreType,
        int score)
    {
        ScoreType = scoreType;
        Score = score;
    }

    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public ScoreType ScoreType { get; set; }

    public int Score { get; set; }
}
