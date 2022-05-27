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

            AssignBillingPeriod(cataloguePrice);
            AssignQuantityCalculationType(cataloguePrice);
        }

        public IEnumerable<SelectListItem> AvailableProvisioningTypes => new SelectListItem[]
        {
            new(ProvisioningType.Patient.Name(), ProvisioningType.Patient.ToString()),
            new(ProvisioningType.PerServiceRecipient.Name(), ProvisioningType.PerServiceRecipient.ToString()),
            new(ProvisioningType.Declarative.Name(), ProvisioningType.Declarative.ToString()),
            new(ProvisioningType.OnDemand.Name(), ProvisioningType.OnDemand.ToString()),
        };

        public IEnumerable<SelectListItem> AvailableBillingPeriods => new List<SelectListItem>
        {
            new("None", null, true),
            new(TimeUnit.PerMonth.Description(), TimeUnit.PerMonth.ToString(), false),
            new(TimeUnit.PerYear.Description(), TimeUnit.PerYear.ToString(), false),
        };

        public IEnumerable<SelectableRadioOption<CataloguePriceCalculationType>> AvailableCalculationTypes =>
            new List<SelectableRadioOption<CataloguePriceCalculationType>>
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

        public IEnumerable<SelectableRadioOption<CataloguePriceQuantityCalculationType>>
            AvailableQuantityCalculationTypes => new List<SelectableRadioOption<CataloguePriceQuantityCalculationType>>
        {
            new(
                CataloguePriceQuantityCalculationType.PerSolutionOrService.Name(),
                CataloguePriceQuantityCalculationType.PerSolutionOrService),
            new(
                CataloguePriceQuantityCalculationType.PerServiceRecipient.Name(),
                CataloguePriceQuantityCalculationType.PerServiceRecipient),
        };

        public int? CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public ProvisioningType? SelectedProvisioningType { get; set; }

        public TimeUnit? PerServiceRecipientBillingPeriod { get; set; }

        public TimeUnit? DeclarativeBillingPeriod { get; set; }

        public TimeUnit? OnDemandBillingPeriod { get; set; }

        public CataloguePriceQuantityCalculationType? DeclarativeQuantityCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? OnDemandQuantityCalculationType { get; set; }

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

        public TimeUnit? GetBillingPeriod()
            => SelectedProvisioningType!.Value switch
            {
                ProvisioningType.Patient => TimeUnit.PerYear,
                ProvisioningType.PerServiceRecipient when PerServiceRecipientBillingPeriod.HasValue => PerServiceRecipientBillingPeriod.Value,
                ProvisioningType.Declarative when CatalogueItemType == CatalogueItemType.AssociatedService => null,
                ProvisioningType.Declarative when DeclarativeBillingPeriod.HasValue => DeclarativeBillingPeriod.Value,
                ProvisioningType.OnDemand when OnDemandBillingPeriod.HasValue => OnDemandBillingPeriod.Value,
                _ => null,
            };

        public CataloguePriceQuantityCalculationType? GetQuantityCalculationType()
            => SelectedProvisioningType!.Value switch
            {
                ProvisioningType.Declarative => DeclarativeQuantityCalculationType,
                ProvisioningType.OnDemand => OnDemandQuantityCalculationType,
                _ => null,
            };

        private void AssignBillingPeriod(CataloguePrice cataloguePrice)
        {
            switch (SelectedProvisioningType)
            {
                case ProvisioningType.OnDemand:
                    OnDemandBillingPeriod = cataloguePrice.TimeUnit;
                    break;
                case ProvisioningType.Declarative:
                    DeclarativeBillingPeriod = cataloguePrice.TimeUnit;
                    break;
                case ProvisioningType.PerServiceRecipient:
                    PerServiceRecipientBillingPeriod = cataloguePrice.TimeUnit;
                    break;
            }
        }

        private void AssignQuantityCalculationType(CataloguePrice cataloguePrice)
        {
            switch (SelectedProvisioningType)
            {
                case ProvisioningType.OnDemand:
                    OnDemandQuantityCalculationType = cataloguePrice.CataloguePriceQuantityCalculationType;
                    break;
                case ProvisioningType.Declarative:
                    DeclarativeQuantityCalculationType = cataloguePrice.CataloguePriceQuantityCalculationType;
                    break;
            }
        }
    }
}
