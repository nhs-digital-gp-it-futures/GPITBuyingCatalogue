using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class FeaturesModel
    {
        public FeaturesModel()
        {
        }

        public FeaturesModel(CatalogueItem catalogueItem)
        {
            var features = new string[0];

            if (!string.IsNullOrWhiteSpace(catalogueItem.Solution.Features))
            {
                features = JsonConvert.DeserializeObject<string[]>(catalogueItem.Solution.Features);

                Listing1 = features.Length > 0 ? features[0] : string.Empty;
                Listing2 = features.Length > 1 ? features[1] : string.Empty;
                Listing3 = features.Length > 2 ? features[2] : string.Empty;
                Listing4 = features.Length > 3 ? features[3] : string.Empty;
                Listing5 = features.Length > 4 ? features[4] : string.Empty;
                Listing6 = features.Length > 5 ? features[5] : string.Empty;
                Listing7 = features.Length > 6 ? features[6] : string.Empty;
                Listing8 = features.Length > 7 ? features[7] : string.Empty;
                Listing9 = features.Length > 8 ? features[8] : string.Empty;
                Listing10 = features.Length > 9 ? features[9] : string.Empty;
            }
        }

        public string Id { get; set; }

        public string Listing1 { get; set; }
        public string Listing2 { get; set; }
        public string Listing3 { get; set; }
        public string Listing4 { get; set; }
        public string Listing5 { get; set; }
        public string Listing6 { get; set; }
        public string Listing7 { get; set; }
        public string Listing8 { get; set; }
        public string Listing9 { get; set; }
        public string Listing10 { get; set; }
    }
}
