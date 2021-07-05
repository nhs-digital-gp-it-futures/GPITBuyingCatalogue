using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class PlugInsOrExtensionsModel : MarketingBaseModel
    {
        public PlugInsOrExtensionsModel()
            : base(null)
        {
        }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(PlugInsRequired);

        public string PlugInsRequired { get; set; }
    }
}
