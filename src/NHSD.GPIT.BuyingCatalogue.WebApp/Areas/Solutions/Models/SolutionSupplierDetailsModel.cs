using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionSupplierDetailsModel : SolutionDisplayBaseModel
    {
        public SolutionSupplierDetailsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            Summary = catalogueItem.Supplier?.Summary;
            Name = catalogueItem.Supplier?.Name;
            Url = catalogueItem.Supplier?.SupplierUrl;

            Contacts = catalogueItem
                .Solution
                .MarketingContacts
                .Select(mc => new SupplierContactViewModel(mc))
                .ToList();
        }

        public SolutionSupplierDetailsModel()
        {
        }

        public override int Index => 12;

        public string Summary { get; }

        public string Name { get; }

        public string Url { get; }

        public List<SupplierContactViewModel> Contacts { get; init; }

        public bool HasContacts() => Contacts?.Any(c => c != null) == true;
    }
}
