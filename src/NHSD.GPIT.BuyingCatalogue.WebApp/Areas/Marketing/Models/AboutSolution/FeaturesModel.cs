using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class FeaturesModel : MarketingBaseModel
    {
        public FeaturesModel()
            : base(null)
        {
        }

        public FeaturesModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";

            var features = CatalogueItem.Solution.GetFeatures();
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

        public override bool? IsComplete =>
            !string.IsNullOrEmpty(Listing1) ||
            !string.IsNullOrEmpty(Listing2) ||
            !string.IsNullOrEmpty(Listing3) ||
            !string.IsNullOrEmpty(Listing4) ||
            !string.IsNullOrEmpty(Listing5) ||
            !string.IsNullOrEmpty(Listing6) ||
            !string.IsNullOrEmpty(Listing7) ||
            !string.IsNullOrEmpty(Listing8) ||
            !string.IsNullOrEmpty(Listing9) ||
            !string.IsNullOrEmpty(Listing10);

        [StringLength(100)]
        public string Listing1 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing2 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing3 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing4 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing5 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing6 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing7 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing8 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing9 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Listing10 { get; set; } = string.Empty;

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Highlight to buyers the key features of your Catalogue Solution.",
                Title = "Catalogue Solution Features",
            };
    }
}
