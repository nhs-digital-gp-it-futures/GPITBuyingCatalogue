using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public override int Index => 5;

        public IList<AssociatedServiceModel> Services { get; set; }
    }
}
