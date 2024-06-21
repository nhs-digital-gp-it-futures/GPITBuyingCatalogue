using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

public class AddEditNhsAppIntegrationModel : NavBaseModel
{
    public AddEditNhsAppIntegrationModel()
    {
    }

    public AddEditNhsAppIntegrationModel(
        CatalogueItem solution,
        IEnumerable<IntegrationType> integrationTypes)
    {
        SolutionName = solution.Name;
        SolutionId = solution.Id;

        var solutionInteroperability = solution.Solution.Integrations;
        NhsAppIntegrations = integrationTypes.Select(
                x => new SelectOption<int>(
                    x.Name,
                    x.Id,
                    solutionInteroperability.Any(
                        y => x.Id == y.IntegrationTypeId)))
            .ToList();
    }

    public CatalogueItemId? SolutionId { get; init; }

    public string SolutionName { get; }

    public List<SelectOption<int>> NhsAppIntegrations { get; set; }
}
