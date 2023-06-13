using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class SolutionJustificationModel
{
    public SolutionJustificationModel()
    {
    }

    public SolutionJustificationModel(
        CatalogueItem solution,
        string justification)
    {
        SolutionId = solution.Id;
        SolutionName = solution.Name;
        SupplierName = solution.Supplier.Name;
        Justification = justification;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public string SupplierName { get; set; }

    [StringLength(1000)]
    public string Justification { get; set; }
}
