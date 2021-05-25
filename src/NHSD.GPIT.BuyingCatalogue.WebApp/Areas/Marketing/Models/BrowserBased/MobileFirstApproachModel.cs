namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class MobileFirstApproachModel : MarketingBaseModel
    {
        public MobileFirstApproachModel()
            : base(null)
        {
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(MobileFirstApproach);

        public string MobileFirstApproach { get; set; }
    }
}
