using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceDetailsModel : NavBaseModel
    {
        public EditAssociatedServiceDetailsModel()
        {
        }

        public EditAssociatedServiceDetailsModel(CatalogueItem solution, CatalogueItem associatedServiceItem)
        {
            AssociatedService = associatedServiceItem;
            Name = associatedServiceItem.Name;
            Description = associatedServiceItem.AssociatedService.Description;
            OrderGuidance = associatedServiceItem.AssociatedService.OrderGuidance;
        }

        public CatalogueItem AssociatedService { get; }

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
