using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels
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
            Description = ClientApplication?.HardwareRequirements;
        }

        [StringLength(500)]
        public string Description { get; set; }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(Description);
    }
}
