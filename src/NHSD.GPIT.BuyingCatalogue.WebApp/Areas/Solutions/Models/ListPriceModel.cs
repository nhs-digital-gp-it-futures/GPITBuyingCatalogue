using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ListPriceModel : SolutionDisplayBaseModel
    {
        public ListPriceModel()
            : base()
        {
        }

        public ListPriceModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus)
        {
            FlatListPrices = item.CataloguePrices
                .Where(cp =>
                    cp.CataloguePriceType == CataloguePriceType.Flat
                    && cp.PublishedStatus == PublicationStatus.Published)
                .Select(cp => new PriceViewModel(cp)).ToList();

            IsFoundation = item.Solution.FrameworkSolutions.Any(fs => fs.IsFoundation).ToYesNo();
            SupplierName = item.Supplier.Name;
        }

        public string SupplierName { get; }

        public string IsFoundation { get; }

        public override int Index => 2;

        public IList<PriceViewModel> FlatListPrices { get; set; }

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Frameworks"
            : "Framework";

        public bool HasFlatListPrices() => FlatListPrices?.Any() == true;
    }
}
