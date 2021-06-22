using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AdditionalServicesModel : SolutionDisplayBaseModel
    {
        public override int Index => 4;

        public IList<AdditionalServiceModel> Services { get; set; }
    }
}
