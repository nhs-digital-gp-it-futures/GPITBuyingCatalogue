using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyStandardsModel : CommunityPharmacySolutionDisplayBaseModel, IStandardsModel
{
    public CommunityPharmacyStandardsModel(
        CatalogueItem catalogueItem,
        IEnumerable<StandardComplianceModel> standards,
        IEnumerable<string> standardsWithWorkOffPlans,
        CatalogueItemContentStatus contentStatus)
        : base(catalogueItem, contentStatus)
    {
        Standards = standards
            .Where(x => x.Compliance is StandardCompliance.InProgress or StandardCompliance.FullyMet)
            .ToList();

        StandardsWithWorkOffPlans = standardsWithWorkOffPlans;
    }

    public override int Index => 3;

    public IList<StandardComplianceModel> Standards { get; init; }

    public IEnumerable<string> StandardsWithWorkOffPlans { get; init; }
}
