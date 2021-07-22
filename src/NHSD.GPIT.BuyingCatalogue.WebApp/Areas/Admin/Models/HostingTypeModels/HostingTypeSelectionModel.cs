using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public class HostingTypeSelectionModel : HostingTypeSectionModel
    {
        public HostingTypeSelectionModel()
            : base()
        {
        }

        public HostingTypeSelectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
        }

        [Required(ErrorMessage = "Select a hosting type")]
        public string SelectedHostingType { get; set; }
    }
}
