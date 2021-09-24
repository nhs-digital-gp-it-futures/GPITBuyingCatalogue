namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionFeaturesModel : SolutionDisplayBaseModel
    {
        public SolutionFeaturesModel(string[] features)
        {
            Features = features;
        }

        public string[] Features { get; }

        public override int Index => 1;
    }
}
