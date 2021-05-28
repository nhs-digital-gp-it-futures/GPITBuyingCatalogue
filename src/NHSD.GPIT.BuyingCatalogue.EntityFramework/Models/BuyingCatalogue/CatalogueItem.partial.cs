using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class CatalogueItem
    {
        public MarketingContact FirstContact() => Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public virtual string Framework() =>
            Solution?.FrameworkSolutions?.FirstOrDefault() is not { } frameSolution
                ? null
                : frameSolution.Framework?.Name;

        public virtual bool? IsFoundation() => Solution?.FrameworkSolutions?.Any(f => f.IsFoundation);

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();
    }
}
