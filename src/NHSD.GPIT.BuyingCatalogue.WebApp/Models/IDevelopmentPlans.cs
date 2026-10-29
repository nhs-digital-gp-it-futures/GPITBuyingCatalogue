using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IDevelopmentPlans
{
    public string RoadmapLink { get; init; }

    public IList<WorkOffPlan> WorkOffPlans { get; init; }
}
