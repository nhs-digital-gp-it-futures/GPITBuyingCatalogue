namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionAssociatedServicesModel : SolutionDisplayBaseModel
    {
        public string Description { get; set; }

        public override int Index => 5;

        public string OrderGuidance { get; set; }
    }
}
