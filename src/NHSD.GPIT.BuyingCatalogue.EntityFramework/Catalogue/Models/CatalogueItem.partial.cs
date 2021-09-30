using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public partial class CatalogueItem
    {
        public virtual CatalogueItemCapability CatalogueItemCapability(
            int capabilityId) =>
            CatalogueItemCapabilities?.FirstOrDefault(
                sc => sc.Capability != null && sc.Capability.Id == capabilityId)
            ?? new CatalogueItemCapability { CatalogueItemId = Id };

        public virtual string[] Features() =>
            HasFeatures() ? JsonSerializer.Deserialize<string[]>(Solution.Features, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) : null;

        public MarketingContact FirstContact() =>
            Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public virtual IList<string> Frameworks() =>
            Solution?.FrameworkSolutions?.Any() == true
                ? Solution?.FrameworkSolutions.Select(s => s.Framework?.ShortName).ToList()
                : new List<string>();

        public virtual bool HasAdditionalServices() => AdditionalService != null
            || Supplier?.CatalogueItems?.Any(c => c.AdditionalService is not null) == true;

        public virtual bool HasAssociatedServices() => AssociatedService != null
            || Supplier?.CatalogueItems?.Any(c => c.AssociatedService is not null) == true;

        public virtual bool HasCapabilities() => CatalogueItemCapabilities?.Any() == true;

        public virtual bool HasClientApplication() => !string.IsNullOrWhiteSpace(Solution?.ClientApplication);

        public virtual bool HasDevelopmentPlans() => !string.IsNullOrWhiteSpace(Solution?.RoadMap);

        public virtual bool HasFeatures() => !string.IsNullOrWhiteSpace(Solution?.Features);

        public virtual bool HasHosting() => Solution?.Hosting is not null;

        public virtual bool HasImplementationDetail() => !string.IsNullOrWhiteSpace(Solution?.ImplementationDetail);

        public virtual bool HasInteroperability() => !string.IsNullOrWhiteSpace(Solution?.Integrations);

        public virtual bool HasListPrice() => CataloguePrices?.Any() == true;

        public virtual bool HasServiceLevelAgreement() => !string.IsNullOrWhiteSpace(Solution?.ServiceLevelAgreement);

        public virtual bool HasSupplierDetails() => Supplier != null;

        public virtual bool? IsFoundation() => Solution?.FrameworkSolutions?.Any(f => f.IsFoundation);

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();

        public string CatalogueItemName(CatalogueItemId catalogueItemId) => Supplier?.CatalogueItems
            .FirstOrDefault(c => c.Id == catalogueItemId)
            ?.Name;

        public string AdditionalServiceDescription(CatalogueItemId catalogueItemId) => Supplier?.CatalogueItems
            .FirstOrDefault(c => c.Id == catalogueItemId)
            ?.AdditionalService?.FullDescription;

        public IEnumerable<CataloguePrice> DuplicateListPrices(
            ProvisioningType provisioningType,
            decimal? price,
            string unitDescription,
            TimeUnit? timeUnit)
            => CataloguePrices.Where(cp =>
                    string.Equals(cp.PricingUnit.Description, unitDescription)
                    && cp.Price == price
                    && cp.ProvisioningType == provisioningType
                    && cp.TimeUnit == timeUnit);
    }
}
