using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class CatalogueItem
    {
        public MarketingContact FirstContact() => Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();
    }
}