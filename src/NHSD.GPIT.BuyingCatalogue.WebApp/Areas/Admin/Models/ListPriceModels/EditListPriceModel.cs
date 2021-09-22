using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
        }

        public EditListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(catalogueItem)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            Price = cataloguePrice.Price.ToString();
            Unit = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;
            SelectedProvisioningType = cataloguePrice.ProvisioningType.ToString();

            if (cataloguePrice.ProvisioningType == ProvisioningType.Declarative)
            {
                DeclarativeTimeUnit = cataloguePrice.TimeUnit;
            }
            else if (cataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                OnDemandTimeUnit = cataloguePrice.TimeUnit;
            }
        }

        public int? CataloguePriceId { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public IEnumerable<SelectListItem> TimeUnitSelectListItems => new SelectListItem[]
        {
            new SelectListItem("per month", TimeUnit.PerMonth.ToString()),
            new SelectListItem("per year", TimeUnit.PerYear.ToString()),
        };

        public IEnumerable<SelectListItem> ProvisioningTypeListItems => new SelectListItem[]
        {
            new SelectListItem("Patient", ProvisioningType.Patient.ToString()),
            new SelectListItem("Declarative", ProvisioningType.Declarative.ToString()),
            new SelectListItem("On Demand", ProvisioningType.OnDemand.ToString()),
        };

        [StringLength(100)]
        [RegularExpression(@"^\d+.?\d{0,4}$", ErrorMessage = "Price must be a number and supports a max of up to 4 decimal places.")]
        public string Price { get; init; }

        [Required]
        [StringLength(100)]
        public string Unit { get; init; }

        [StringLength(1000)]
        public string UnitDefinition { get; init; }

        public string SelectedProvisioningType { get; init; }

        public TimeUnit? DeclarativeTimeUnit { get; init; }

        public TimeUnit? OnDemandTimeUnit { get; init; }

        public PricingUnit GetPricingUnit()
        {
            var localUnitDescription = Unit;
            if (!localUnitDescription.StartsWith("per"))
                localUnitDescription = string.Format("per {0}", localUnitDescription);

            return new PricingUnit
            {
                Name = localUnitDescription.Replace("per", string.Empty).Replace(" ", string.Empty),
                TierName = localUnitDescription.Replace("per ", string.Empty),
                Description = localUnitDescription,
                Definition = UnitDefinition,
            };
        }

        public bool TryGetProvisioningType(out ProvisioningType provisioningType)
        {
            if (!Enum.TryParse(SelectedProvisioningType, out provisioningType))
                return false;

            return true;
        }

        public TimeUnit? GetTimeUnit(ProvisioningType provisioningType)
        {
            if (provisioningType == ProvisioningType.Patient)
                return TimeUnit.PerYear;

            return provisioningType == ProvisioningType.Declarative
                ? DeclarativeTimeUnit.Value
                : OnDemandTimeUnit.Value;
        }

        public bool TryParsePrice(out decimal price)
        {
            if (!decimal.TryParse(Price, out price))
            {
                return false;
            }

            return true;
        }
    }
}
