using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;

public class CatalogueSolutionStandardsModel : NavBaseModel
{
    public CatalogueSolutionStandardsModel(
        CatalogueItemId solutionId,
        string solutionName,
        IEnumerable<StandardComplianceModel> standards)
    {
        SolutionId = solutionId;
        SolutionName = solutionName;

        var groupedStandards = standards.GroupBy(x => x.Type).ToList();

        OverarchingStandards = groupedStandards.Where(x => x.Key == StandardType.Overarching).SelectMany(x => x).ToList();
        OtherStandards = groupedStandards.Where(x => x.Key == StandardType.Other).SelectMany(x => x).ToList();
        SupplementaryCareStandards = groupedStandards.Where(x => x.Key == StandardType.SupplementaryCare).SelectMany(x => x).ToList();
    }

    public CatalogueItemId SolutionId { get; }

    public string SolutionName { get; }

    public ICollection<StandardComplianceModel> OverarchingStandards { get; }

    public ICollection<StandardComplianceModel> OtherStandards { get; }

    public ICollection<StandardComplianceModel> SupplementaryCareStandards { get; }
}
