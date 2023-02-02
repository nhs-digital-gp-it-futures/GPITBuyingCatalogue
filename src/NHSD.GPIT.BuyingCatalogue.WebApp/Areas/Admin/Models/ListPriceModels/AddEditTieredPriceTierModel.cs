using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class AddEditTieredPriceTierModel : NavBaseModel
    {
        public AddEditTieredPriceTierModel()
        {
        }

        public AddEditTieredPriceTierModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            Title = "Add a pricing tier";
        }

        public AddEditTieredPriceTierModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice, CataloguePriceTier cataloguePriceTier)
            : this(catalogueItem, cataloguePrice)
        {
            TierId = cataloguePriceTier.Id;
            InputPrice = $"{cataloguePriceTier.Price}";
            LowerRange = cataloguePriceTier.LowerRange;
            UpperRange = cataloguePriceTier.UpperRange;
            IsInfiniteRange = cataloguePriceTier.UpperRange == null;
            Title = "Edit a pricing tier";
        }

        public static IList<SelectOption<bool>> AvailableRangeOptions => new List<SelectOption<bool>>
        {
            new("Infinite upper range", true),
            new("Set upper range", false),
        };

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public int CataloguePriceId { get; set; }

        public int? TierId { get; set; }

        public decimal? Price => InputPrice.AsNullableDecimal();

        public string InputPrice { get; set; }

        public int? LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public bool? IsInfiniteRange { get; set; }

        public bool? IsEditing { get; set; }

        public string DeleteTieredPriceTierUrl { get; set; }
    }
}
