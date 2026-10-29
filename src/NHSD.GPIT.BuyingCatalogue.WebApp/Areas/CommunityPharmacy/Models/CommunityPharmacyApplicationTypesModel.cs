using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyApplicationTypesModel : CommunityPharmacySolutionDisplayBaseModel, IApplicationTypesDisplayModel
{
    public CommunityPharmacyApplicationTypesModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        ApplicationTypesDetails = new ApplicationTypesDetailsModel(catalogueItem.Solution.EnsureAndGetApplicationType());

        PaginationFooter.FullWidth = true;
    }

    public override int Index => 5;

    public ApplicationTypesDetailsModel ApplicationTypesDetails { get; }
}
