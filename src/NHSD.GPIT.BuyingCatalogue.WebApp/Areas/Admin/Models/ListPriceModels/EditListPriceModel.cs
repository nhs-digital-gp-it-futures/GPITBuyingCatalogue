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
            Price = cataloguePrice.Price;
            Unit = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;
            SelectedProvisioningType = cataloguePrice.ProvisioningType;

            if (cataloguePrice.ProvisioningType == ProvisioningType.Declarative)
            {
                DeclarativeTimeUnit = cataloguePrice.TimeUnit;
            }
            else if (cataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
            {
                OnDemandTimeUnit = cataloguePrice.TimeUnit;
            }
        }

        public static IEnumerable<SelectListItem> TimeUnitSelectListItems => new SelectListItem[]
        {
            new("per month", TimeUnit.PerMonth.ToString()),
            new("per year", TimeUnit.PerYear.ToString()),
        };

        public static IEnumerable<SelectListItem> ProvisioningTypeListItems => new SelectListItem[]
        {
            new("Patient", ProvisioningType.Patient.ToString()),
            new("Declarative", ProvisioningType.Declarative.ToString()),
            new("On Demand", ProvisioningType.OnDemand.ToString()),
        };

        public int? CataloguePriceId { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public decimal? Price { get; init; }

        [StringLength(100)]
        public string Unit { get; init; }

        [StringLength(1000)]
        public string UnitDefinition { get; init; }

        public ProvisioningType? SelectedProvisioningType { get; init; }

        public TimeUnit? DeclarativeTimeUnit { get; init; }

        public TimeUnit? OnDemandTimeUnit { get; init; }

        public PricingUnit GetPricingUnit()
            => new()
            {
                Description = Unit,
                Definition = UnitDefinition,
            };

        public TimeUnit? GetTimeUnit(ProvisioningType provisioningType)
        {
            if (provisioningType == ProvisioningType.Patient)
                return TimeUnit.PerYear;

            if (provisioningType == ProvisioningType.Declarative && DeclarativeTimeUnit.HasValue)
                return DeclarativeTimeUnit.Value;
            else if (provisioningType == ProvisioningType.OnDemand && OnDemandTimeUnit.HasValue)
                return OnDemandTimeUnit.Value;

            return null;
        }
    }
}
