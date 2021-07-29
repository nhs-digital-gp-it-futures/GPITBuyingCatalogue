using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels
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

        public override bool IsComplete => !string.IsNullOrWhiteSpace(AdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
