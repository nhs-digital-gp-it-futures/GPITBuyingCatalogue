using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class PricingDashboardModel : NavBaseModel
{
    public PricingDashboardModel(
        Competition competition)
    {
        CompetitionId = competition.Id;
        CompetitionName = competition.Name;
        SolutionPrices = competition.CompetitionSolutions.Select(x => new SolutionPriceModel(x, competition)).ToList();
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public ICollection<SolutionPriceModel> SolutionPrices { get; set; }
}
