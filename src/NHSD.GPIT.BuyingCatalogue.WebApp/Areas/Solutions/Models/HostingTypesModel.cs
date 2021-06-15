using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class HostingTypesModel : SolutionDisplayBaseModel
    {
        public PublicCloud PublicCloud { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public HybridHostingType HybridHostingType { get; set; }

        public OnPremise OnPremise { get; set; }

        public override int Index => 9;
    }
}
