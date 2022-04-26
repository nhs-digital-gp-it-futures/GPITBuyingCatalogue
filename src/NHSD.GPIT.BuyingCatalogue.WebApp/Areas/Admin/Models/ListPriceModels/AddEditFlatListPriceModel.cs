using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class AddEditFlatListPriceModel : NavBaseModel
    {
        public AddEditFlatListPriceModel()
        {
        }

        public AddEditFlatListPriceModel(CatalogueItem catalogueItem)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;

            CataloguePricePublicationStatus = PublicationStatus.Draft;
        }

        public AddEditFlatListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(catalogueItem)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            UnitDescription = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;

            var flatTier = cataloguePrice.CataloguePriceTiers.First();
            Price = flatTier.Price;

            SelectedPublicationStatus = CataloguePricePublicationStatus = cataloguePrice.PublishedStatus;
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
                "Volume",
                "Buyers pay the same price for all units based on how many they buy.",
                CataloguePriceCalculationType.Volume),
        };

        public PublicationStatus CataloguePricePublicationStatus { get; set; }

        public int? CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public ProvisioningType? SelectedProvisioningType { get; set; }

        public TimeUnit? DeclarativeBillingPeriod { get; set; }

        public TimeUnit? OnDemandBillingPeriod { get; set; }

        public CataloguePriceCalculationType? SelectedCalculationType { get; set; }

        public decimal? Price { get; set; }

        [StringLength(100)]
        public string UnitDescription { get; set; }

        [StringLength(1000)]
        public string UnitDefinition { get; set; }

        public PublicationStatus? SelectedPublicationStatus { get; set; }

        public IList<SelectableRadioOption<PublicationStatus>> AvailablePublicationStatuses => CataloguePricePublicationStatus
            .GetAvailablePublicationStatuses()
            .Select(p => new SelectableRadioOption<PublicationStatus>(p.Description(), p))
            .ToList();

        public PricingUnit GetPricingUnit()
            => new()
            {
                Description = UnitDescription,
                Definition = UnitDefinition,
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
