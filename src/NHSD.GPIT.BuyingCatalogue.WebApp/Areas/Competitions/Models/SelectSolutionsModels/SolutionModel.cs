using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class SolutionModel
{
    public SolutionModel()
    {
    }

    public SolutionModel(
        CatalogueItemId solutionId,
        string solutionName,
        string supplierName,
        List<string> requiredServices,
        bool isSelected = false)
    {
        SolutionId = solutionId;
        SolutionName = solutionName;
        SupplierName = supplierName;
        RequiredServices = requiredServices;
        Selected = isSelected;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public string SupplierName { get; set; }

    public List<string> RequiredServices { get; set; } = new();

    public bool Selected { get; set; }
}
