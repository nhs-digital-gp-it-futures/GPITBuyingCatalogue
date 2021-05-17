using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel()
            : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            OperatingSystems = ProfileDefaults.SupportedOperatingSystems;

            CheckOperatingSystems();

            Description = ClientApplication?.MobileOperatingSystems?.OperatingSystemsDescription;
        }

        public override bool? IsComplete => ClientApplication?.MobileOperatingSystems?.OperatingSystems?.Any();

        public SupportedOperatingSystemModel[] OperatingSystems { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        private void CheckOperatingSystems()
        {
            foreach (var browser in OperatingSystems)
            {
                if (ClientApplication?.MobileOperatingSystems?.OperatingSystems != null &&
                    ClientApplication.MobileOperatingSystems.OperatingSystems.Any(x => x.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    browser.Checked = true;
                }
            }
        }
    }
}
