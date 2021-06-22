using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class AssociatedServicesModel : SolutionDisplayBaseModel
    {
        public override int Index => 5;

        public IList<AssociatedServiceModel> Services { get; set; } = new List<AssociatedServiceModel>();

        public bool HasServices() => Services != null && Services.Any();
    }
}
