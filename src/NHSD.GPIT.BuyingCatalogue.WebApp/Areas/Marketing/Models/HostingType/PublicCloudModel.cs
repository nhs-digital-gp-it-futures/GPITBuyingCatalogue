using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PublicCloudModel : MarketingBaseModel
    {
        public PublicCloudModel()
            : base(null)
        {
        }

        public override bool? IsComplete => PublicCloud?.IsValid();

        public PublicCloud PublicCloud { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(PublicCloud?.RequiresHscn);
            set => PublicCloud.RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }
    }
}
