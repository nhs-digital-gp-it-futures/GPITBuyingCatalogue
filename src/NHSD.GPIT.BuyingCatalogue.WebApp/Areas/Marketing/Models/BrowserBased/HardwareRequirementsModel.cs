using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class HardwareRequirementsModel : MarketingBaseModel
    {
        public HardwareRequirementsModel()
            : base(null)
        {
        }

        [StringLength(500)]
        public string Description { get; set; }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Description);
    }
}
