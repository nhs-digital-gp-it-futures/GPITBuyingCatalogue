using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServicesModel : SolutionDisplayBaseModel
    {
        public override int Index => 4;

        public IList<AdditionalServiceModel> Services { get; set; }
    }
}
