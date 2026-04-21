using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;

public class CatalogueSolutionStandardModel : NavBaseModel
{
    public CatalogueSolutionStandardModel()
    {
    }

    public CatalogueSolutionStandardModel(
        CatalogueItemId solutionId,
        string solutionName,
        StandardComplianceModel standard)
    {
        SolutionId = solutionId;
        SolutionName = solutionName;
        StandardName = standard.Name;
        Compliance = standard.Compliance;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public string StandardName { get; set; }

    public StandardCompliance Compliance { get; set; }

    public List<SelectOption<StandardCompliance>> ComplianceOptions =>
    [
        new(
            StandardCompliance.FullyMet.Description(),
            StandardCompliance.FullyMet),

        new(
            StandardCompliance.InProgress.Description(),
            StandardCompliance.InProgress),

    ];
}
