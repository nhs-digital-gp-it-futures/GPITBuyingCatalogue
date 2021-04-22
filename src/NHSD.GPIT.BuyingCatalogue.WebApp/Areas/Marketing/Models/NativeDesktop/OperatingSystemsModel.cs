using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel() : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            OperatingSystemsDescription = ClientApplication.NativeDesktopOperatingSystemsDescription;
        }

        public override bool? IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopOperatingSystemsDescription); }
        }        

        public string OperatingSystemsDescription { get; set; }
    }
}
