using System.Linq;
using System.Text.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public partial class CatalogueItem
    {
        public bool IsBrowsable => PublishedStatus switch
        {
            PublicationStatus.Published => true,
            PublicationStatus.InRemediation => true,
            _ => false,
        };

        public virtual string[] Features() =>
            !string.IsNullOrWhiteSpace(Solution?.Features)
            ? JsonSerializer.Deserialize<string[]>(Solution.Features, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        public MarketingContact FirstContact() =>
            Solution?.MarketingContacts?.FirstOrDefault() ?? new MarketingContact();

        public MarketingContact SecondContact() =>
            Solution?.MarketingContacts?.Skip(1).FirstOrDefault() ?? new MarketingContact();

        public string CatalogueItemName(CatalogueItemId catalogueItemId) => Supplier?.CatalogueItems
            .FirstOrDefault(c => c.Id == catalogueItemId)
            ?.Name;

        public string AdditionalServiceDescription(CatalogueItemId catalogueItemId) => Supplier?.CatalogueItems
            .FirstOrDefault(c => c.Id == catalogueItemId)
            ?.AdditionalService?.FullDescription;
    }
}
