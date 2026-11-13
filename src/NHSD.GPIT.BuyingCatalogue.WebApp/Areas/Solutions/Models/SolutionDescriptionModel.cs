using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionDescriptionModel : SolutionDisplayBaseModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, SolutionSections sections)
            : base(catalogueItem, contentStatus, sections)
        {
            var solution = catalogueItem.Solution;

            Description = solution.FullDescription;
            Summary = solution.Summary;
            AboutUrl = catalogueItem.Solution.AboutUrl;
            Contacts = catalogueItem.Supplier.SupplierContacts.ToList();
            SupplierUrl = catalogueItem.Supplier.SupplierUrl;
        }

        public static IList<string> NhsAssurance =>
        [
            "Pharmacy first", "Pharmacy contraception", "Blood pressure check", "Smoking cessation", "Discharge medicine",
            "New medicines",
        ];

        public string Description { get; init; }

        public string AboutUrl { get; init; }

        public override int Index => 0;

        public string Summary { get; init; }

        public IList<SupplierContact> Contacts { get; init; }

        public string SupplierUrl { get; init; }

        public bool HasDescription() => !string.IsNullOrWhiteSpace(Description);

        public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);

        public bool HasAboutUrl() => !string.IsNullOrWhiteSpace(AboutUrl);

        public bool HasContactDetails() => Contacts is not null && Contacts.Count > 0;

        public bool HasSupplierUrl() => !string.IsNullOrWhiteSpace(SupplierUrl);
    }
}
