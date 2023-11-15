using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

public class AddEditNhsAppIntegrationModel : NavBaseModel
{
    public AddEditNhsAppIntegrationModel()
    {
    }

    public AddEditNhsAppIntegrationModel(CatalogueItem solution)
    {
        SolutionName = solution.Name;
        SolutionId = solution.Id;

        var solutionInteroperability = solution.Solution.GetIntegrations();
        NhsAppIntegrations = Interoperability.NhsAppIntegrations.Select(
                x => new SelectOption<string>(
                    x,
                    x,
                    solutionInteroperability.Any(
                        y => string.Equals(y.IntegrationType, Interoperability.NhsAppIntegrationType)
                            && string.Equals(y.Qualifier, x))))
            .ToList();
    }

    public CatalogueItemId? SolutionId { get; init; }

    public string SolutionName { get; }

    public List<SelectOption<string>> NhsAppIntegrations { get; set; }
}
