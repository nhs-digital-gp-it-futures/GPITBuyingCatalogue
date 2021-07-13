using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class CapabilitiesViewModel : SolutionDisplayBaseModel, INoNavModel
    {
        public string CapabilitiesHeading { get; set; }

        public override int Index => 2;

        public string Name { get; set; }

        public IList<RowViewModel> RowViewModels { get; set; } = new List<RowViewModel>();
    }
}
