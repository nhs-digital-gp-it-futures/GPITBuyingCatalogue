using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class SolutionModel
{
    public SolutionModel()
    {
    }

    public SolutionModel(
        CatalogueItem solution,
        List<string> requiredServices,
        bool isSelected = false)
    {
        SolutionId = solution.Id;
        SolutionName = solution.Name;
        SupplierName = solution.Supplier.Name;
        RequiredServices = requiredServices;
        Selected = isSelected;
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
