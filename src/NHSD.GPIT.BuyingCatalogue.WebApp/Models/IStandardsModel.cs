using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IStandardsModel
{
    public IList<StandardComplianceModel> Standards { get; init; }

    public IEnumerable<string> StandardsWithWorkOffPlans { get; init; }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }
}
