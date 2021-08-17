using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public sealed class ThirdPartyComponentsModel : ApplicationTypeBaseModel
    {
        public ThirdPartyComponentsModel()
        {
        }

        public ThirdPartyComponentsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";

            ThirdPartyComponents = ClientApplication?.MobileThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = ClientApplication?.MobileThirdParty?.DeviceCapabilities;
        }

        public override bool IsComplete =>
            !string.IsNullOrWhiteSpace(ThirdPartyComponents) ||
            !string.IsNullOrWhiteSpace(DeviceCapabilities);

        [StringLength(500)]
        public string ThirdPartyComponents { get; set; }

        [StringLength(500)]
        public string DeviceCapabilities { get; set; }
    }
}
