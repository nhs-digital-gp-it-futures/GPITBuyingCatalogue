using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyApplicationTypesModel : CommunityPharmacySolutionDisplayBaseModel, IApplicationTypesDisplayModel
{
    public CommunityPharmacyApplicationTypesModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        var applicationTypes = new ApplicationTypesModel(catalogueItem, contentStatus);
        ApplicationTypeDetail = applicationTypes.ApplicationTypeDetail;
        ApplicationTypes = applicationTypes.ApplicationTypes;
        BrowserBasedApplication = applicationTypes.BrowserBasedApplication;
        NativeDesktopApplication = applicationTypes.NativeDesktopApplication;
        NativeMobileApplication = applicationTypes.NativeMobileApplication;

        PaginationFooter.FullWidth = true;
    }

    public override int Index => 5;

    [UIHint("DescriptionList")]
    public DescriptionListViewModel ApplicationTypes { get; init; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel BrowserBasedApplication { get; init; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel NativeDesktopApplication { get; init; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel NativeMobileApplication { get; init; }

    public ApplicationTypeDetail ApplicationTypeDetail { get; init; }
}
