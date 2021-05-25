using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class AdditionalInformationModel : MarketingBaseModel
    {
        public AdditionalInformationModel()
            : base(null)
        {
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(AdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
