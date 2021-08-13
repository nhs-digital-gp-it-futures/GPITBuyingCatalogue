using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
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
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";
            AdditionalInformation = ClientApplication?.NativeMobileAdditionalInformation;
        }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(AdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
