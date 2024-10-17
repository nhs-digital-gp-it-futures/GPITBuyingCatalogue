using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class ShortlistedSolutionModel(SolutionModel solution, string frameworkName, bool selectBox = false) : NavBaseModel
{
    public string FrameworkName { get; set; } = frameworkName;

    public SolutionModel Solution { get; set; } = solution;
}
