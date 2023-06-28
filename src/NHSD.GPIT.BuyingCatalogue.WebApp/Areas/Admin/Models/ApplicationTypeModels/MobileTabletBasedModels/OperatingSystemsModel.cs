using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels
{
    public sealed class OperatingSystemsModel : ApplicationTypeBaseModel
    {
        public OperatingSystemsModel()
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            OperatingSystems = new SupportedOperatingSystemModel[]
            {
                new() { OperatingSystemName = "Apple IOS" },
                new() { OperatingSystemName = "Android" },
                new() { OperatingSystemName = "Other" },
            };

            CheckOperatingSystems();

            Description = ApplicationTypeDetail?.MobileOperatingSystems?.OperatingSystemsDescription;
        }

        public SupportedOperatingSystemModel[] OperatingSystems { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        private void CheckOperatingSystems()
        {
            foreach (var browser in OperatingSystems)
            {
                browser.Checked =
                    (ApplicationTypeDetail?.MobileOperatingSystems?.OperatingSystems ?? new HashSet<string>()).Any(
                        s => s.Equals(browser.OperatingSystemName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
