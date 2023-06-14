using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels
{
    public class AdditionalInformationModel : ApplicationTypeBaseModel
    {
        public AdditionalInformationModel()
            : base()
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            AdditionalInformation = ClientApplication?.AdditionalInformation;
        }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
