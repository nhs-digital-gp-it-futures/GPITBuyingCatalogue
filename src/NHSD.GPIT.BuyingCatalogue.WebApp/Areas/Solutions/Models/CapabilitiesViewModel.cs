using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class CapabilitiesViewModel : SolutionDisplayBaseModel
    {
        public IList<RowViewModel> RowViewModels { get; set; } = new List<RowViewModel>();

        public override int Index => 2;
    }
}
