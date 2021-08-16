using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public class HardwareRequirementsModel : ApplicationTypeBaseModel
    {
        public HardwareRequirementsModel()
            : base()
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";
            Description = ClientApplication?.NativeMobileHardwareRequirements;
        }

        [StringLength(500)]
        public string Description { get; set; }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(Description);
    }
}
