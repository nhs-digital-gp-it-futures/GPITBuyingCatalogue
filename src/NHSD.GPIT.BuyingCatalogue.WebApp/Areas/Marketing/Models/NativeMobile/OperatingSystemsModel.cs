using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel() : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            OperatingSystems = new SupportedOperatingSystemModel[]
            {
                new SupportedOperatingSystemModel{ OperatingSystemName = "Apple IOS" },
                new SupportedOperatingSystemModel{ OperatingSystemName = "Android" },
                new SupportedOperatingSystemModel{ OperatingSystemName = "Other" }
            };

            CheckOperatingSystems();

            Description = ClientApplication.MobileOperatingSystems?.OperatingSystemsDescription;
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.MobileOperatingSystems?.OperatingSystems?.Any(); }
        }

        public SupportedOperatingSystemModel[] OperatingSystems { get; set; }

        public string Description { get; set; }
        
        private void CheckOperatingSystems()
        {
            foreach (var browser in OperatingSystems)
            {
                if (ClientApplication.MobileOperatingSystems?.OperatingSystems != null &&
                    ClientApplication.MobileOperatingSystems.OperatingSystems.Any(x => x.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    browser.Checked = true;
                }
            }
        }
    }
}
