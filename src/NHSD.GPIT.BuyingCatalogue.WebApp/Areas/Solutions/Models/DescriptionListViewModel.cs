using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class DescriptionListViewModel
    {
        public IDictionary<string, ListViewModel> Items { get; set; } = new Dictionary<string, ListViewModel>();

        public bool HasValues() => Items.Any();
    }
}
