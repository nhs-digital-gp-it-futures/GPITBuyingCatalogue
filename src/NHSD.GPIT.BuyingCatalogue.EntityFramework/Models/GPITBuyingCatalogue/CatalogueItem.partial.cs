using System.Linq;
using Newtonsoft.Json;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class CatalogueItem
    {
        public virtual string[] Features() =>
            HasFeatures() ? JsonConvert.DeserializeObject<string[]>(Solution.Features) : null;

        public MarketingContact FirstContact() => Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public virtual string Framework() =>
            Solution?.FrameworkSolutions?.FirstOrDefault() is not { } frameSolution
                ? null
                : frameSolution.Framework?.Name;

        public bool HasCapabilities() => Solution?.SolutionCapabilities?.Any() == true;

        public bool HasClientApplication() => !string.IsNullOrWhiteSpace(Solution?.ClientApplication);

        public bool HasFeatures() => !string.IsNullOrWhiteSpace(Solution?.Features);

        public bool HasHosting() => !string.IsNullOrWhiteSpace(Solution?.Hosting);

        public bool HasImplementationDetail() => !string.IsNullOrWhiteSpace(Solution?.ImplementationDetail);

        public bool HasServiceLevelAgreement() => !string.IsNullOrWhiteSpace(Solution?.ServiceLevelAgreement);

        public virtual bool? IsFoundation() => Solution?.FrameworkSolutions?.Any(f => f.IsFoundation);

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();
    }
}
