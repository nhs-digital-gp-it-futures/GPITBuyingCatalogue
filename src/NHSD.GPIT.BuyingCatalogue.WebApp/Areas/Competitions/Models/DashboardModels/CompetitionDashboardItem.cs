using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class CompetitionDashboardItem
{
    public CompetitionDashboardItem(
        int id,
        string name,
        string description,
        DateTime lastUpdated,
        DateTime? completedDate)
    {
        Id = id;
        Name = name;
        Description = description;
        LastUpdated = lastUpdated;
        Progress = completedDate.HasValue ? TaskProgress.Completed : TaskProgress.InProgress;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime LastUpdated { get; set; }

    public TaskProgress Progress { get; set; }
}
