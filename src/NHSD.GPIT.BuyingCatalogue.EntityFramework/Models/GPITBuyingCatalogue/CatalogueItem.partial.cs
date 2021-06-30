using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class CatalogueItem
    {
        public virtual string[] Features() =>
            HasFeatures() ? JsonConvert.DeserializeObject<string[]>(Solution.Features) : null;

        public MarketingContact FirstContact() =>
            Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public virtual IList<string> Frameworks() =>
            Solution?.FrameworkSolutions?.Any() == true
                ? Solution?.FrameworkSolutions.Select(s => s.Framework?.ShortName).ToList()
                : new List<string>();

        public virtual bool HasAdditionalServices() =>
            Supplier?.CatalogueItems?.Select(c => c.AdditionalService).Any() == true;

        public virtual bool HasAssociatedServices() => AssociatedService != null
            || Supplier?.CatalogueItems?.Any(c => c.AssociatedService is not null) == true;

        public virtual bool HasCapabilities() => Solution?.SolutionCapabilities?.Any() == true;

        public virtual bool HasClientApplication() => !string.IsNullOrWhiteSpace(Solution?.ClientApplication);

        public virtual bool HasDevelopmentPlans() => !string.IsNullOrWhiteSpace(Solution?.RoadMap);

        public virtual bool HasFeatures() => !string.IsNullOrWhiteSpace(Solution?.Features);

        public virtual bool HasHosting() => !string.IsNullOrWhiteSpace(Solution?.Hosting);

        public virtual bool HasImplementationDetail() => !string.IsNullOrWhiteSpace(Solution?.ImplementationDetail);

        public virtual bool HasInteroperability() => !string.IsNullOrWhiteSpace(Solution?.Integrations);

        public virtual bool HasListPrice() => CataloguePrices?.Any() == true;

        public virtual bool HasServiceLevelAgreement() => !string.IsNullOrWhiteSpace(Solution?.ServiceLevelAgreement);

        public virtual bool HasSupplierDetails() => Supplier != null;

        public virtual bool? IsFoundation() => Solution?.FrameworkSolutions?.Any(f => f.IsFoundation);

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();
    }
}
