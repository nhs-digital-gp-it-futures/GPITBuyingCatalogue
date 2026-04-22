using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;

public struct StandardPartialModel(
    CatalogueItemId solutionId,
    StandardType standardType,
    ICollection<StandardComplianceModel> standards)
{
    public CatalogueItemId SolutionId { get; set; } = solutionId;

    public StandardType StandardType { get; set; } = standardType;

    public ICollection<StandardComplianceModel> Standards { get; set; } = standards;
}
