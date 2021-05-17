using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PublicCloudModel : MarketingBaseModel
    {
        public PublicCloudModel()
            : base(null)
        {
        }

        public PublicCloudModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
            PublicCloud = catalogueItem.Solution.GetHosting().PublicCloud;
        }

        public override bool? IsComplete => PublicCloud?.IsValid();

        public PublicCloud PublicCloud { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(PublicCloud?.RequiresHscn);
            set => PublicCloud.RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }
    }
}
