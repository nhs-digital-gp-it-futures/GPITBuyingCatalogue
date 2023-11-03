using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class CompetitionDashboardItem
{
    public CompetitionDashboardItem(
        Competition competition)
    {
        Id = competition.Id;
        Name = competition.Name;
        Description = competition.Description;
        Solutions = competition.CompetitionSolutions;
        LastUpdated = competition.LastUpdated;
        ShortlistAccepted = competition.ShortlistAccepted;
        Progress = competition.Completed.HasValue ? TaskProgress.Completed : TaskProgress.InProgress;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public IEnumerable<CompetitionSolution> Solutions { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime? ShortlistAccepted { get; set; }

    public TaskProgress Progress { get; set; }

    public bool IsDirectAward() => Solutions.Count() == 1;
}
