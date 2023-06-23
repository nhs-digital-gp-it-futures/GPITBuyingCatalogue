using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SharedModels;

public class SolutionModel
{
    public SolutionModel()
    {
    }

    public SolutionModel(
        CompetitionSolution competitionSolution)
    {
        SolutionId = competitionSolution.Solution.CatalogueItemId;
        SolutionName = competitionSolution.Solution.CatalogueItem.Name;
        SupplierName = competitionSolution.Solution.CatalogueItem.Supplier.Name;
        RequiredServices = competitionSolution.RequiredServices.Select(y => y.Service.CatalogueItem.Name).ToList();
        Selected = competitionSolution.IsShortlisted;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public string SupplierName { get; set; }

    public List<string> RequiredServices { get; set; } = new();

    public bool Selected { get; set; }

    public string GetAdditionalServicesList() => RequiredServices.Any()
        ? string.Join(", ", RequiredServices)
        : "None";
}
