using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class AddAssociatedServiceModel : NavBaseModel
    {
        public AddAssociatedServiceModel()
        {
        }

        public AddAssociatedServiceModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/associated-services";
            Solution = catalogueItem;
        }

        public CatalogueItem Solution { get; }

        [Required(ErrorMessage = "Enter a name")]
        [StringLength(300)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter order guidance")]
        [StringLength(1000)]
        public string OrderGuidance { get; set; }
    }
}
