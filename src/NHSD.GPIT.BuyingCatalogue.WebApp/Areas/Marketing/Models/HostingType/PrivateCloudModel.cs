using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PrivateCloudModel : MarketingBaseModel
    {
        public PrivateCloudModel()
            : base(null)
        {
        }

        public override bool? IsComplete => PrivateCloud?.IsValid();

        public PrivateCloud PrivateCloud { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(PrivateCloud?.RequiresHscn);
            set => PrivateCloud.RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }
    }
}
