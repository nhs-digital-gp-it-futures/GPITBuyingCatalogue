using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ListPriceModel : SolutionDisplayBaseModel
    {
        public ListPriceModel()
            : base()
        {
        }

        public ListPriceModel(CatalogueItem item, CatalogueItem service, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus, true)
        {
            FlatListPrices = service.CataloguePrices
                .Where(p =>
                    p.CataloguePriceType == CataloguePriceType.Flat
                    && p.PublishedStatus == PublicationStatus.Published).ToList();

            TieredListPrices = service.CataloguePrices
                .Where(p =>
                    p.CataloguePriceType == CataloguePriceType.Tiered
                    && p.PublishedStatus == PublicationStatus.Published).ToList();

            ItemType = service.CatalogueItemType;
            PriceFor = service.CatalogueItemType.Name();
            Title = PriceFor + " prices";
        }

        public ListPriceModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus, false)
        {
            FlatListPrices = item.CataloguePrices
                .Where(p =>
                    p.CataloguePriceType == CataloguePriceType.Flat
                    && p.PublishedStatus == PublicationStatus.Published).ToList();

            TieredListPrices = item.CataloguePrices
                .Where(p =>
                    p.CataloguePriceType == CataloguePriceType.Tiered
                    && p.PublishedStatus == PublicationStatus.Published).ToList();

            ItemType = item.CatalogueItemType;
            IndexValue = 2;
            PriceFor = "Catalogue Solution";
            SetPaginationFooter();
        }

        public int IndexValue { get; set; } = 2;

        public string PriceFor { get; set; }

        public CatalogueItemType ItemType { get; set; }

        public override int Index => IndexValue;

        public IList<CataloguePrice> FlatListPrices { get; set; }

        public IList<CataloguePrice> TieredListPrices { get; set; }

        public bool HasFlatListPrices() => FlatListPrices?.Any() == true;
    }
}
