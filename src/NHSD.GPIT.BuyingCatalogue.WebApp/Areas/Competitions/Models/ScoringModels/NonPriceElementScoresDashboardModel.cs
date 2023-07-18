using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class NonPriceElementScoresDashboardModel : NavBaseModel
{
    public NonPriceElementScoresDashboardModel()
    {
    }

    public NonPriceElementScoresDashboardModel(Competition competition)
    {
        CompetitionName = competition.Name;
        NonPriceElements = competition.NonPriceElements.GetNonPriceElements()
            .ToDictionary(x => x, x => TaskProgress.NotStarted);
    }

    public string CompetitionName { get; set; }

    public Dictionary<NonPriceElement, TaskProgress> NonPriceElements { get; set; }
}
