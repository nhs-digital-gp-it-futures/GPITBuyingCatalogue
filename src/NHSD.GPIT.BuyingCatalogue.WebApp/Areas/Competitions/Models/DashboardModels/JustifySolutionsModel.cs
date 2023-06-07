using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class JustifySolutionsModel : NavBaseModel
{
    public JustifySolutionsModel()
    {
    }

    public JustifySolutionsModel(string competitionName, IEnumerable<CompetitionSolution> nonShortlistedSolutions)
    {
        CompetitionName = competitionName;
        Solutions = nonShortlistedSolutions.Select(
                x => new SolutionJustificationModel(x.Solution.CatalogueItem))
            .OrderBy(x => x.SolutionName)
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<SolutionJustificationModel> Solutions { get; set; }
}

public class SolutionJustificationModel
{
    public SolutionJustificationModel()
    {
    }

    public SolutionJustificationModel(
        CatalogueItem solution)
    {
        SolutionId = solution.Id;
        SolutionName = solution.Name;
        SupplierName = solution.Supplier.Name;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public string SupplierName { get; set; }

    [StringLength(1000)]
    public string Justification { get; set; }
}
