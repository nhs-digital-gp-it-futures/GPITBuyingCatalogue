using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class AddTieredListPriceModel : NavBaseModel
    {
        public AddTieredListPriceModel()
        {
        }

        public AddTieredListPriceModel(CatalogueItem catalogueItem)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
        }

        public AddTieredListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(catalogueItem)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            UnitDescription = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;
            RangeDefinition = cataloguePrice.PricingUnit.RangeDescription;

            SelectedCalculationType = cataloguePrice.CataloguePriceCalculationType;
            SelectedProvisioningType = cataloguePrice.ProvisioningType;

            if (SelectedProvisioningType == ProvisioningType.OnDemand)
                OnDemandBillingPeriod = cataloguePrice.TimeUnit;
            else if (SelectedProvisioningType == ProvisioningType.Declarative)
                DeclarativeBillingPeriod = cataloguePrice.TimeUnit;
        }

        public IEnumerable<SelectListItem> AvailableProvisioningTypes => new SelectListItem[]
        {
            new(ProvisioningType.Patient.Name(), ProvisioningType.Patient.ToString()),
            new(ProvisioningType.Declarative.Name(), ProvisioningType.Declarative.ToString()),
            new(ProvisioningType.OnDemand.Name(), ProvisioningType.OnDemand.ToString()),
        };

        public IList<SelectListItem> AvailableBillingPeriods => new List<SelectListItem>
        {
            new(TimeUnit.PerMonth.Description(), TimeUnit.PerMonth.ToString()),
            new(TimeUnit.PerYear.Description(), TimeUnit.PerYear.ToString()),
        };

        public IList<SelectableRadioOption<CataloguePriceCalculationType>> AvailableCalculationTypes => new List<SelectableRadioOption<CataloguePriceCalculationType>>
        {
            new(
                CataloguePriceCalculationType.SingleFixed.Name(),
                CataloguePriceCalculationType.SingleFixed.Description(),
                CataloguePriceCalculationType.SingleFixed),
            new(
                CataloguePriceCalculationType.Cumulative.Name(),
                CataloguePriceCalculationType.Cumulative.Description(),
                CataloguePriceCalculationType.Cumulative),
            new(
                CataloguePriceCalculationType.Volume.Name(),
                CataloguePriceCalculationType.Volume.Description(),
                CataloguePriceCalculationType.Volume),
        };

        public int? CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public ProvisioningType? SelectedProvisioningType { get; set; }

        public TimeUnit? DeclarativeBillingPeriod { get; set; }

        public TimeUnit? OnDemandBillingPeriod { get; set; }

        public CataloguePriceCalculationType? SelectedCalculationType { get; set; }

        [StringLength(100)]
        public string UnitDescription { get; set; }

        [StringLength(1000)]
        public string UnitDefinition { get; set; }

        [StringLength(100)]
        public string RangeDefinition { get; set; }

        public PricingUnit GetPricingUnit()
            => new()
            {
                Description = UnitDescription,
                Definition = UnitDefinition,
                RangeDescription = RangeDefinition,
            };

        public TimeUnit? GetTimeUnit()
            => SelectedProvisioningType!.Value switch
            {
                ProvisioningType.Patient => TimeUnit.PerYear,
                ProvisioningType.Declarative when CatalogueItemType == CatalogueItemType.AssociatedService => null,
                ProvisioningType.Declarative when DeclarativeBillingPeriod.HasValue => DeclarativeBillingPeriod.Value,
                ProvisioningType.OnDemand when OnDemandBillingPeriod.HasValue => OnDemandBillingPeriod.Value,
                _ => null,
            };
    }
}
