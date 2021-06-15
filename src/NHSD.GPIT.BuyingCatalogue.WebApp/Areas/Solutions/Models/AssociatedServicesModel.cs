using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public override int Index => 9;
    }
}
