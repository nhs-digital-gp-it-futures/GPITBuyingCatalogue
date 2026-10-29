using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class SummaryModel : CommunityPharmacySolutionDisplayBaseModel
{
    public SummaryModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus)
        : base(catalogueItem, contentStatus)
    {
        Summary = catalogueItem.Solution.Summary;
        Contacts = catalogueItem.Supplier.SupplierContacts.ToList();
        SupplierUrl = catalogueItem.Supplier.SupplierUrl;
    }

    public static IList<string> NhsAssurance =>
    [
        "Pharmacy first", "Pharmacy contraception", "Blood pressure check", "Smoking cessation", "Discharge medicine",
        "New medicines",
    ];

    public override int Index => 0;

    public string Summary { get; init; }

    public IList<SupplierContact> Contacts { get; init; }

    public string SupplierUrl { get; init; }

    public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);

    public bool HasContactDetails() => Contacts is not null && Contacts.Count > 0;

    public bool HasSupplierUrl() => !string.IsNullOrWhiteSpace(SupplierUrl);
}
