using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacySupplierDetails : CommunityPharmacySolutionDisplayBaseModel, ISupplierDetailsModel
{
    public CommunityPharmacySupplierDetails(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        Summary = catalogueItem.Supplier?.Summary;
        Name = catalogueItem.Supplier?.Name;
        Url = catalogueItem.Supplier?.SupplierUrl;

        Contacts = catalogueItem.Supplier?.SupplierContacts
            .Select(mc => new SupplierContactViewModel(mc))
            .ToList();
    }

    public override int Index => 8;

    public string Summary { get; }

    public string Name { get; }

    public string Url { get; }

    public List<SupplierContactViewModel> Contacts { get; init; }

    public bool HasContacts() => Contacts?.Any(c => c != null) == true;
}
