using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public sealed class EditListPriceModel : NavBaseModel
    {
        public EditListPriceModel()
        {
        }

        public EditListPriceModel(CatalogueItem catalogueItem)
        {
            CatalogueItemType = catalogueItem.CatalogueItemType;
            ItemId = catalogueItem.Id;
            ItemName = catalogueItem.Name;
            Title = "Add a list price";
        }

        public EditListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(catalogueItem)
        {
            CatalogueItemType = catalogueItem.CatalogueItemType;
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            Price = cataloguePrice.Price;
            Unit = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;
            SelectedProvisioningType = cataloguePrice.ProvisioningType;
            Title = $"{catalogueItem.Name} list price";

            if (CatalogueItemType == CatalogueItemType.Solution && cataloguePrice.ProvisioningType == ProvisioningType.Declarative)
            {
                DeclarativeTimeUnit = cataloguePrice.TimeUnit;
            }
            else if (cataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                OnDemandTimeUnit = cataloguePrice.TimeUnit;
            }
        }

        public EditListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice, CatalogueItemId relatedCatalogueItemId)
            : this(catalogueItem, cataloguePrice)
        {
            RelatedCatalogueItemId = relatedCatalogueItemId;
        }

        public static IEnumerable<SelectListItem> TimeUnitSelectListItems => new SelectListItem[]
        {
            new("per month", TimeUnit.PerMonth.ToString()),
            new("per year", TimeUnit.PerYear.ToString()),
        };

        public static IEnumerable<SelectListItem> ProvisioningTypeListItems => new SelectListItem[]
        {
            new(ProvisioningType.Patient.Name(), ProvisioningType.Patient.ToString()),
            new(ProvisioningType.Declarative.Name(), ProvisioningType.Declarative.ToString()),
            new(ProvisioningType.OnDemand.Name(), ProvisioningType.OnDemand.ToString()),
        };

        public CatalogueItemType CatalogueItemType { get; init; }

        public int? CataloguePriceId { get; init; }

        public CatalogueItemId ItemId { get; init; }

        public string ItemName { get; init; }

        public ProvisioningType? SelectedProvisioningType { get; init; }

        public TimeUnit? DeclarativeTimeUnit { get; init; }

        public TimeUnit? OnDemandTimeUnit { get; init; }

        public decimal? Price { get; init; }

        [StringLength(100)]
        public string Unit { get; init; }

        [StringLength(1000)]
        public string UnitDefinition { get; init; }

        public string Title { get; init; }

        public string DeleteLink { get; init; }

        public CatalogueItemId RelatedCatalogueItemId { get; }

        public PricingUnit GetPricingUnit()
            => new()
            {
                Description = Unit,
                Definition = UnitDefinition,
            };

        public TimeUnit? GetTimeUnit(ProvisioningType provisioningType)
            => provisioningType switch
            {
                ProvisioningType.Patient => TimeUnit.PerYear,
                ProvisioningType.Declarative when CatalogueItemType != CatalogueItemType.Solution => null,
                ProvisioningType.Declarative when DeclarativeTimeUnit.HasValue => DeclarativeTimeUnit.Value,
                ProvisioningType.OnDemand when OnDemandTimeUnit.HasValue => OnDemandTimeUnit.Value,
                _ => null,
            };
    }
}
