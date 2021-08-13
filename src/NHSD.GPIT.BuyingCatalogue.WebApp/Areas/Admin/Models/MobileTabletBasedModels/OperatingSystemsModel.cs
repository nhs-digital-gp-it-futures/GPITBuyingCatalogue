using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public class OperatingSystemsModel : ApplicationTypeBaseModel
    {
        public OperatingSystemsModel()
            : base()
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";

            OperatingSystems = new SupportedOperatingSystemModel[]
            {
                new() { OperatingSystemName = "Apple IOS" },
                new() { OperatingSystemName = "Android" },
                new() { OperatingSystemName = "Other" },
            };

            CheckOperatingSystems();

            Description = ClientApplication?.MobileOperatingSystems?.OperatingSystemsDescription;
        }

        public override bool IsComplete => OperatingSystems.Any();

        public SupportedOperatingSystemModel[] OperatingSystems { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        private void CheckOperatingSystems()
        {
            foreach (var browser in OperatingSystems)
            {
                browser.Checked =
                    (ClientApplication?.MobileOperatingSystems?.OperatingSystems ?? new HashSet<string>()).Any(
                        s => s.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
